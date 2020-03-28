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
                    Settings = new IntegrationTestSettings
                    {
                        Authority = $@"{azureAdOptions.Authority}/v2.0",
                        ApplicationIdUri = azureAdOptions.ApplicationIdUri,
                        AppAuthentication = appAuthentication,
                        UserAuthentication = userAuthentication
                    };
                });
            });
            Client = _webAppFactory.CreateDefaultClient();
        }
    }
}
