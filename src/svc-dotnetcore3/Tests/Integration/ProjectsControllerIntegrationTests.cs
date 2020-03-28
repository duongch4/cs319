// using Web.API;
// using Web.API.Application.Communication;
// using Web.API.Resources;
// using System;
// using System.Text;
// using System.IO;
// using System.Collections.Generic;
// using System.Net.Http;
// using System.Net.Http.Headers;
using System.Threading.Tasks;
using Tests.Integration.Utils;
using Xunit;

using StatusCodes = System.Net.HttpStatusCode;
using Newtonsoft.Json;

using Serilog;

namespace Tests.Integration
{
    public class ProjectsControllerIntegrationTests : IntegrationTestBase
    {
        public ProjectsControllerIntegrationTests(AppFixture app) : base(app)
        {
        }

        [Fact]
        public async Task GetAllProjects()
        {
            // var settings = Settings;
            var response = await Client.GetAsync("/api/projects");
            Assert.Equal(StatusCodes.Unauthorized, response.StatusCode);
            // Assert.Equal(StatusCodes.Status401Unauthorized, (int)response.StatusCode);

            // response.EnsureSuccessStatusCode(); // Status Code: 200-299

            // var responseString = await response.Content.ReadAsStringAsync();
            // var responseJSON = JsonConvert.DeserializeObject<OkResponse<IEnumerable<ProjectSummary>>>(responseString);

            // Assert.Contains("Google", responseString);
        }
    }
}
