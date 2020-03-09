using Web.API;
using Web.API.Application.Communication;
using Web.API.Resources;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;

namespace Tests.Integration
{
    public class ProjectsControllerIntegrationTests : IClassFixture<WebApplicationFactory<Startup>> 
    { 
        private readonly HttpClient _client; 
        public ProjectsControllerIntegrationTests(WebApplicationFactory<Startup> factory) 
        {
            _client = factory.CreateClient(); 
        }

        [Fact] public async Task GetAllProjects() 
        { 
            var response = await _client.GetAsync("/api/projects"); 

            response.EnsureSuccessStatusCode(); 

            var responseString = await response.Content.ReadAsStringAsync();
            var responseJSON = JsonConvert.DeserializeObject<OkResponse<IEnumerable<ProjectSummary>>>(responseString);
            // var responseJSON = JsonConvert.DeserializeObject<object>(responseString);

            Assert.Contains("Google", responseString);
        }
    }
}
