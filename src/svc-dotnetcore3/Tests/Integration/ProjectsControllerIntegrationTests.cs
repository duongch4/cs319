using Tests.Integration.Utils;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System;
using Newtonsoft.Json;
using Web.API.Resources;
using Web.API.Application.Communication;
using Xunit;

namespace Tests.Integration
{
    [TestCaseOrderer("Tests.Integration.Utils.PriorityOrderer", "Tests")]
    public class ProjectsControllerIntegrationTests : IntegrationTestBase
    {
        private static readonly string createdTitle = "Created-Title";
        private static readonly string updatedTitle = "Updated-Title";
        private static readonly string managerId = "c14b2f4d-a8f0-4c35-b8d6-5c657cdc76b5";
        // private static readonly string notManagerId = "c14b2f4d-a8f0-4c35-b8d6-5c657cdc76b5";
        public ProjectsControllerIntegrationTests(AppFixture app) : base(app)
        { }

        [Theory, TestPriority(0)]
        [InlineData("/api/projects", "1234-5678")]
        public async Task CreateOneProject_Pass(string url, string projectNumber)
        {
            var projectProfile = JsonConvert.SerializeObject(GetProjectProfile(managerId, projectNumber, createdTitle));
            var req = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(
                    projectProfile,
                    Encoding.UTF8,
                    "application/json"
                )
            };
            await AccessTokenProvider.AuthenticateRequestAsAppAsync(req);
            var res = await Client.SendAsync(req);
            Assert.Equal(HttpStatusCode.Created, res.StatusCode);
        }

        [Theory, TestPriority(1)]
        [InlineData("/api/projects", "1234-5678")]
        public async Task GetOneProject_AfterCreate_Pass(string url, string projectNumber)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, $"{url}/{projectNumber}");
            await AccessTokenProvider.AuthenticateRequestAsAppAsync(req);
            var res = await Client.SendAsync(req);
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            string jsonString = await res.Content.ReadAsStringAsync();
            var jsonObject = JsonConvert.DeserializeObject<OkResponse<ProjectProfile>>(jsonString);
            Assert.Equal(createdTitle, jsonObject.payload.ProjectSummary.Title);
        }

        [Theory, TestPriority(2)]
        [InlineData("/api/projects", "1234-5678")]
        public async Task UpdateOneProject_Pass(string url, string projectNumber)
        {
            var projectProfile = JsonConvert.SerializeObject(GetProjectProfile(managerId, projectNumber, updatedTitle));
            var req = new HttpRequestMessage(HttpMethod.Put, $"{url}/{projectNumber}")
            {
                Content = new StringContent(
                    projectProfile,
                    Encoding.UTF8,
                    "application/json"
                )
            };
            await AccessTokenProvider.AuthenticateRequestAsAppAsync(req);
            var res = await Client.SendAsync(req);
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        }

        [Theory, TestPriority(3)]
        [InlineData("/api/projects", "1234-5678")]
        public async Task GetOneProject_AfterUpdate_Pass(string url, string projectNumber)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, $"{url}/{projectNumber}");
            await AccessTokenProvider.AuthenticateRequestAsAppAsync(req);
            var res = await Client.SendAsync(req);
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            string jsonString = await res.Content.ReadAsStringAsync();
            var jsonObject = JsonConvert.DeserializeObject<OkResponse<ProjectProfile>>(jsonString);
            Assert.Equal(updatedTitle, jsonObject.payload.ProjectSummary.Title);
        }

        [Theory, TestPriority(4)]
        [InlineData("/api/projects", "1234-5678")]
        public async Task DeleteOneProject_Pass(string url, string projectNumber)
        {
            var req = new HttpRequestMessage(HttpMethod.Delete, $"{url}/{projectNumber}");
            await AccessTokenProvider.AuthenticateRequestAsAppAsync(req);
            var res = await Client.SendAsync(req);
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        }

        private static IEnumerable<object[]> GetData_POST(int numTests)
        {
            var allData = new List<object[]>
            {
                new object[] { 1, 2, 3 },
                new object[] { -4, -6, -10 },
                new object[] { -2, 2, 0 },
                new object[] { int.MinValue, -1, int.MaxValue },
            };

            return allData.Take(numTests);
        }

        private static ProjectManager GetProjectManager(string id, string firstName, string lastName)
        {
            return new ProjectManager
            {
                UserID = id,
                FirstName = firstName,
                LastName = lastName
            };
        }

        private static ProjectSummary GetProjectSummary(string title, string projectNumber)
        {
            return new ProjectSummary
            {
                Title = title,
                ProjectNumber = projectNumber,
                Location = new LocationResource
                {
                    LocationID = 1,
                    Province = "British Columbia",
                    City = "Vancouver"
                },
                ProjectStartDate = new DateTime(2020, 5, 1),
                ProjectEndDate = new DateTime(2020, 5, 2)
            };
        }

        private static IEnumerable<OpeningPositionsSummary> GetOpenings()
        {
            var opening = new OpeningPositionsSummary
            {
                PositionID = 0,
                CommitmentMonthlyHours = new Dictionary<string, int>{ ["2020-05-01"] = 5 },
                Discipline = "Automation?",
                YearsOfExp = "1-3",
                Skills = new HashSet<string> { "Automated teller machines?", "Digital labor?" }
            };
            return Enumerable.Empty<OpeningPositionsSummary>().Append(opening);
        }

        private static ProjectProfile GetProjectProfile(string managerId, string projectNumber, string title)
        {
            return new ProjectProfile
            {
                ProjectManager = GetProjectManager(managerId, "Clinton", "Barton"),
                ProjectSummary = GetProjectSummary(title, projectNumber),
                UsersSummary = Enumerable.Empty<UserSummary>(),
                Openings = GetOpenings()
            };
        }
    }
}