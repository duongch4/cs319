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

using System.Net.Http;
using System.Net;
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
        public async Task GetAllProjects_NoAuthentication()
        {
            var req = new HttpRequestMessage(HttpMethod.Get, "/api/projects");
            var res = await Client.SendAsync(req);
            Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
        }

        [Fact]
        public async Task GetAllProjects_AppAuthentication()
        {
            var req = new HttpRequestMessage(HttpMethod.Get, "/api/projects");
            await AccessTokenProvider.AuthenticateRequestAsAppAsync(req);
            var res = await Client.SendAsync(req);
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            res.EnsureSuccessStatusCode(); // Status Code: 200-299
            
            // var responseString = await response.Content.ReadAsStringAsync();
            // var responseJSON = JsonConvert.DeserializeObject<OkResponse<IEnumerable<ProjectSummary>>>(responseString);
        }
    }
}
