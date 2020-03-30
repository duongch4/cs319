using Web.API;
using AzureAdOptions = Web.API.Authentication.AzureAdOptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using Xunit;

namespace Tests.Integration.Utils
{
    public class AppFixture : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _webAppFactory;
        public HttpClient Client { get; }
        public IntegrationTestSettings Settings { get; private set; }

        public AppFixture()
        {
            _webAppFactory = new WebApplicationFactory<Startup>().WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("IntegrationTesting");
                builder.ConfigureAppConfiguration(configBuilder =>
                {
                    var config = configBuilder.Build();
                    var azureAdOptions = config.GetSection("AzureAd").Get<AzureAdOptions>();
                    var appAuthentication = config.GetSection("AppAuthenticationSetting").Get<AppAuthenticationSettings>();
                    var userAuthentication = config.GetSection("UserAuthenticationSettings").Get<UserAuthenticationSettings>();
                    var userAdmin = config.GetSection("UserAdmin").Get<UserAuthenticationSettingsDetails>();
                    var userRegular = config.GetSection("UserRegular").Get<UserAuthenticationSettingsDetails>();
                    var userNotInDatabase = config.GetSection("UserNotInDatabase").Get<UserAuthenticationSettingsDetails>();
                    var userNoRoles = config.GetSection("UserNoRoles").Get<UserAuthenticationSettingsDetails>();
                    var wrongRolesScopesComb = config.GetSection("WrongRolesScopesComb").Get<UserAuthenticationSettingsWrongRolesScopesComb>();
                    Settings = new IntegrationTestSettings
                    {
                        Authority = $@"{azureAdOptions.Authority}/v2.0",
                        ApplicationIdUri = azureAdOptions.ApplicationIdUri,
                        AppAuthentication = appAuthentication,
                        UserAuthentication = userAuthentication,
                        UserAdmin = userAdmin,
                        UserRegular = userRegular,
                        UserNotInDatabase = userNotInDatabase,
                        UserNoRoles = userNoRoles,
                        WrongRolesScopesComb = wrongRolesScopesComb
                    };
                });
            });
            Client = _webAppFactory.CreateDefaultClient();
        }
    }
}
