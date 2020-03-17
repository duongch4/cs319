using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System;
using System.Reflection;
using System.IO;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Threading.Tasks;
using Web.API.Application.Repository;
using Web.API.Infrastructure.Config;
using Web.API.Infrastructure.Data;
using AuthenticationOptions = Web.API.Authentication.AuthenticationOptions;
using Web.API.Authorization;
using Web.API.OpenApi;
using Serilog;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;

namespace Web.API
{
    public class AllowAnonymous : IAuthorizationHandler
    {
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            foreach (IAuthorizationRequirement requirement in context.PendingRequirements.ToList())
                context.Succeed(requirement); //Simply pass all requirements

            return Task.CompletedTask;
        }
    }

    public static class LoggerExtensions
    {
        public static ILogger Here(this ILogger logger,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            return logger
                .ForContext("MemberName", memberName)
                .ForContext("FilePath", sourceFilePath)
                .ForContext("LineNumber", sourceLineNumber);
        }
    }

    public class Startup
    {
        private readonly IWebHostEnvironment _environment;
        public IConfiguration _configuration { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _environment = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            var azureAdOptions = _configuration.GetSection("AzureAd").Get<AzureAdOptions>();
            var authenticationOptions = GetAuthenticationOptions(azureAdOptions);

            var connectionString = _configuration["ConnectionString"];
            var allowedOrigins = _configuration["AllowedOrigins"];

            AddCors(services, allowedOrigins);
            AddAuthentication(services, authenticationOptions);
            AddAuthorization(services);

            AddRepositories(services, connectionString);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); // what does this do???

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // //Allows auth to be bypassed for LocalDev
            // if (_environment.IsDevelopment())
            // {
            //     services.AddSingleton<IAuthorizationHandler, AllowAnonymous>();
            // }

            AddSpaStaticFiles(services);

            AddSwagger(services, authenticationOptions);
        }

        private AuthenticationOptions GetAuthenticationOptions(AzureAdOptions azureAdOptions)
        {
            return new AuthenticationOptions
            {
                Authority = $@"{azureAdOptions.Authority}/v2.0",
                AuthorizationUrl = $@"{azureAdOptions.Authority}/oauth2/v2.0/authorize",
                ClientId = $@"{azureAdOptions.ClientId}",
                ApplicationIdUri = $@"api://{azureAdOptions.ClientId}"
            };
        }

        private void AddSpaStaticFiles(IServiceCollection services)
        {
            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ui-react-client/build";
                if (!_environment.IsProduction())
                {
                    configuration.RootPath = "../../ui-react-client/build";
                }
            });
        }

        private void AddRepositories(IServiceCollection services, string connectionString)
        {
            services.AddScoped<IDisciplinesRepository>(sp => new DisciplinesRespository(connectionString));
            services.AddScoped<ILocationsRepository>(sp => new LocationsRepository(connectionString));
            services.AddScoped<IPositionsRepository>(sp => new PositionsRepository(connectionString));
            services.AddScoped<IProjectsRepository>(sp => new ProjectsRepository(connectionString));
            services.AddScoped<IResourceDisciplineRepository>(sp => new ResourceDisciplineRepository(connectionString));
            services.AddScoped<ISkillsRepository>(sp => new SkillsRepository(connectionString));
            services.AddScoped<IUsersRepository>(sp => new UsersRepository(connectionString));
            services.AddScoped<IOutOfOfficeRepository>(sp => new OutOfOfficeRepository(connectionString));
        }

        private void AddAuthentication(IServiceCollection services, AuthenticationOptions authenticationOptions)
        {
            services.Configure<AuthenticationOptions>(options => options = authenticationOptions);

            services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Audience = authenticationOptions.ClientId;
                options.Authority = authenticationOptions.Authority;
            });

            services.AddSingleton<IClaimsTransformation, ScopeClaimSplitTransformation>();
        }

        private static void AddAuthorization(IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                // Require callers to have at least one valid permission by default
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .AddRequirements(new AnyValidPermissionRequirement())
                    .Build();
                // Create a policy for each action that can be done in the API
                foreach (string action in Actions.All)
                {
                    options.AddPolicy(action, policy => policy.AddRequirements(new ActionAuthorizationRequirement(action)));
                }
            });
            services.AddSingleton<IAuthorizationHandler, AnyValidPermissionRequirementHandler>();
            services.AddSingleton<IAuthorizationHandler, ActionAuthorizationRequirementHandler>();
        }

        private void AddCors(IServiceCollection services, string allowedOrigins)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.WithOrigins(allowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                );
            });
        }

        private void AddSwagger(IServiceCollection services, AuthenticationOptions authenticationOptions)
        {
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                var infoV1 = CreateOpenApiInfo(
                    "v1", "Resource Utilization API Version 1",
                    "Detailed layout of Resource Utilization API with ASP.NET Core Web API"
                );
                var infoV2 = CreateOpenApiInfo(
                    "v2", "Resource Utilization API Version 2",
                    "Detailed layout of Resource Utilization API with ASP.NET Core Web API"
                );

                c.SwaggerDoc("v1", infoV1);
                c.SwaggerDoc("v2", infoV2);

                AddSwaggerJsonAndUi(c);

                AddSwaggerSecurityDefinition(c, authenticationOptions);
                AddSwaggerSecurityRequirement(c);
            });
        }

        private void AddSwaggerJsonAndUi(Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions c)
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        }

        private void AddSwaggerSecurityDefinition(Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions c, AuthenticationOptions authenticationOptions)
        {
            c.AddSecurityDefinition("aad-jwt", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(authenticationOptions.AuthorizationUrl),
                        Scopes = DelegatedPermissions.All.ToDictionary(p => $"{authenticationOptions.ApplicationIdUri}/{p}")
                    }
                }
            });
        }

        private void AddSwaggerSecurityRequirement(Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions c)
        {
            c.OperationFilter<OAuthSecurityRequirementOperationFilter>();
        }

        private OpenApiInfo CreateOpenApiInfo(string version, string title, string description)
        {
            return new OpenApiInfo
            {
                Version = version,
                Title = title,
                Description = description,
                TermsOfService = CreateOpenApiTermsOfService(),
                Contact = CreateOpenApiContact(),
                License = CreateOpenApiLicense()
            };
        }

        private Uri CreateOpenApiTermsOfService()
        {
            return new Uri("https://example.com/terms");
        }

        private OpenApiContact CreateOpenApiContact()
        {
            return new OpenApiContact
            {
                Name = "Bang Chi Duong",
                Email = "bangchi.duong.20193@outlook.com",
                Url = new Uri("https://bangchi.tk")
            };
        }

        private OpenApiLicense CreateOpenApiLicense()
        {
            return new OpenApiLicense
            {
                Name = "Use under LICX",
                Url = new Uri("https://example.com/license")
            };
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<AuthenticationOptions> options)
        {
            UseSwagger(app, options);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSerilogRequestLogging(); // <-- Add this line

            app.UseHttpsRedirection();
            app.UseCors("CorsPolicy");
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireAuthorization();
            });

            app.UseSpaStaticFiles();
            UseSpa(app, env);
        }

        private void UseSwagger(IApplicationBuilder app, IOptions<AuthenticationOptions> options)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.SwaggerEndpoint("/swagger/v2/swagger.json", "My API V2");
                // c.RoutePrefix = string.Empty;

                c.OAuthClientId(options.Value.ClientId);
            });
        }

        private void UseSpa(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ui-react-client/";

                if (env.IsDevelopment())
                {
                    spa.Options.SourcePath = "../../ui-react-client/";
                    // spa.UseReactDevelopmentServer(npmScript: "start");
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:3000");
                }
                else if (env.IsStaging())
                {
                    spa.Options.SourcePath = "../../ui-react-client/";
                }
            });
        }
    }
}
