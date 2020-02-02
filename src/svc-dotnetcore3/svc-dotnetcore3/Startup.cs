using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Threading.Tasks;
using Web.API.Application.Repository;
using Web.API.Infrastructure.Config;
using Web.API.Infrastructure.Data;
using Serilog;

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
        private readonly IWebHostEnvironment environment;
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            environment = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.WithOrigins(Configuration["AllowedOrigins"])
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                );
            });

            var connectionString = Configuration["ConnectionString"];

            services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var authSettings = Configuration.GetSection("AzureAd").Get<AzureAdOptions>();

                options.Audience = authSettings.ClientId;
                options.Authority = authSettings.Authority;
            });

            services.AddScoped<ILocationsRepository>(sp => new LocationsRepository(connectionString));
            services.AddScoped<IProjectsRepository>(sp => new ProjectsRepository(connectionString));
            services.AddScoped<IUsersRepository>(sp => new UsersRepository(connectionString));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            //Allows auth to be bypassed for LocalDev
            if (environment.IsDevelopment())
            {
                services.AddSingleton<IAuthorizationHandler, AllowAnonymous>();
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var outputTemplate = "[{Timestamp:HH:mm:ss} {Level}]:{NewLine}  {SourceContext}{NewLine}  {Message}{NewLine}  Method: [{MemberName}] at [{FilePath}:{LineNumber}]:{NewLine}  {Exception}{NewLine}";

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.File(@"Logs\log_.txt", rollingInterval: RollingInterval.Day, outputTemplate: outputTemplate)
                .WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information, outputTemplate: outputTemplate)
                // .WriteTo.MSSqlServer(
                //     loggingConnectionStringBuilder.ConnectionString, 
                //     "Ibex", 
                //     Serilog.Events.LogEventLevel.Information)
                // .WriteTo.AzureAnalytics(
                //     workspaceId: Configuration["AZUREANALYTICS_WORKSPACEID"],
                //     authenticationId: Configuration["AZUREANALYTICS_AUTHENTICATIONID"],
                //     logName: "ibex",
                //     restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Debug, 
                //     batchSize: 10)
                // .WriteTo.Email( 
                //     //notice that we should only email the most urgent messages
                //     restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Fatal, 
                //     connectionInfo: new EmailConnectionInfo()
                //     {
                //         MailServer = "mercury.nbsuply.com",
                //         FromEmail = "serilog@hmwallace.com",
                //         ToEmail = "",
                //         EmailSubject = "Serilog subject",
                //     })
                .CreateLogger();

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
                endpoints.MapControllers();
            });
        }
    }
}
