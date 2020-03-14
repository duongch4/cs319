using Web.API;
using Web.API.Application.Communication;
using Web.API.Resources;
using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

// using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using StatusCodes = Microsoft.AspNetCore.Http.StatusCodes;
using Newtonsoft.Json;

using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Tests.Integration
{
    public class ProjectsControllerIntegrationTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Startup> _factory;
        private Dictionary<string, string> _settings;
        public ProjectsControllerIntegrationTests(WebApplicationFactory<Startup> factory)
        {
            // _settings = new Dictionary<string, string>();
            var projectDir = Directory.GetCurrentDirectory();
            // var configPath = Path.Combine(projectDir, "appsettings.json");
            var configPath = Path.Combine(projectDir, "appsettings.IntegrationTests.json");
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseSetting("Environment", "Development"); // Development to by-pass Auth
                // builder.UseSetting("Environment", "Integration"); // Default is Development which we dont want since we want to test for Authorization
                // builder.ConfigureAppConfiguration((context, config) => {
                //     var AzureAd = config.AddJsonFile(configPath).Build().GetSection("AzureAd");
                //     _settings.Add("Authority", AzureAd.GetValue<string>("Instance") + AzureAd.GetValue<string>("Tenant"));
                //     _settings.Add("Tenant", AzureAd.GetValue<string>("Tenant"));
                //     _settings.Add("ClientId", AzureAd.GetValue<string>("ClientId"));
                // });
            });
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task GetAllProjects()
        {
            var response = await _client.GetAsync("/api/projects");
            // Assert.Equal(StatusCodes.Status401Unauthorized, (int)response.StatusCode);

            response.EnsureSuccessStatusCode(); // Status Code: 200-299

            var responseString = await response.Content.ReadAsStringAsync();
            var responseJSON = JsonConvert.DeserializeObject<OkResponse<IEnumerable<ProjectSummary>>>(responseString);

            Assert.Contains("Google", responseString);
        }
    }
}
