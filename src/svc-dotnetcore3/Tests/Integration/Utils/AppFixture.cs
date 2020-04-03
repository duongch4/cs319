using Web.API;
using AzureAdOptions = Web.API.Authentication.AzureAdOptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using Xunit;

namespace Tests.Integration.Utils
{
    public class AppFixture : IDisposable
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

        private bool _disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _webAppFactory.Dispose();
                }

                _disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
    }
}
