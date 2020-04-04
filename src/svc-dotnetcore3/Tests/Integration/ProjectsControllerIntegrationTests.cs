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
        private static readonly string managerId = "c14b2f4d-a8f0-4c35-b8d6-5c657cdc76b5";
        public ProjectsControllerIntegrationTests(AppFixture app) : base(app)
        { }

        [Theory, TestPriority(0)]
        [MemberData(nameof(Data_POST_PUT), parameters: new object[] { 2, "Created-Title" })]
        public async Task CreateOneProject(string url, string projectNumber, string title, string discipline, HashSet<string> skills)
        {
            var projectProfile = JsonConvert.SerializeObject(GetProjectProfile(managerId, projectNumber, title, discipline, skills));
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
        [MemberData(nameof(Data_GET), parameters: new object[] { 2, "Created-Title" })]
        public async Task GetOneProject_AfterCreate(string url, string projectNumber, string title)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, $"{url}/{projectNumber}");
            await AccessTokenProvider.AuthenticateRequestAsAppAsync(req);
            var res = await Client.SendAsync(req);
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            string jsonString = await res.Content.ReadAsStringAsync();
            var jsonObject = JsonConvert.DeserializeObject<OkResponse<ProjectProfile>>(jsonString);
            Assert.Equal(title, jsonObject.payload.ProjectSummary.Title);
            Assert.Single(jsonObject.payload.Openings);
        }

        [Theory, TestPriority(2)]
        [MemberData(nameof(Data_POST_PUT), parameters: new object[] { 2, "Updated-Title" })]
        public async Task UpdateOneProject(string url, string projectNumber, string title, string discipline, HashSet<string> skills)
        {
            // var discipline = "Automation";
            // var skills = new HashSet<string> { "Automated teller machines", "Digital labor" };
            var projectProfile = JsonConvert.SerializeObject(GetProjectProfile(managerId, projectNumber, title, discipline, skills));
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
        [MemberData(nameof(Data_GET), parameters: new object[] { 2, "Updated-Title" })]
        public async Task GetOneProject_AfterUpdate(string url, string projectNumber, string title)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, $"{url}/{projectNumber}");
            await AccessTokenProvider.AuthenticateRequestAsAppAsync(req);
            var res = await Client.SendAsync(req);
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            string jsonString = await res.Content.ReadAsStringAsync();
            var jsonObject = JsonConvert.DeserializeObject<OkResponse<ProjectProfile>>(jsonString);
            Assert.Equal(title, jsonObject.payload.ProjectSummary.Title);
            Assert.Single(jsonObject.payload.Openings);
        }

        [Theory, TestPriority(4)]
        [MemberData(nameof(Data_DEL_GET_FINAL), parameters: 2)]
        public async Task DeleteOneProject(string url, string projectNumber)
        {
            var req = new HttpRequestMessage(HttpMethod.Delete, $"{url}/{projectNumber}");
            await AccessTokenProvider.AuthenticateRequestAsAppAsync(req);
            var res = await Client.SendAsync(req);
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        }

        [Theory, TestPriority(5)]
        [MemberData(nameof(Data_DEL_GET_FINAL), parameters: 2)]
        public async Task GetOneProject_AfterDelete(string url, string projectNumber)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, $"{url}/{projectNumber}");
            await AccessTokenProvider.AuthenticateRequestAsAppAsync(req);
            var res = await Client.SendAsync(req);
            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
        }

        public static IEnumerable<object[]> Data_POST_PUT(int numTests, string title)
        {
            var url = "/api/projects";
            // var createdTitle = "Created-Title";
            // var updatedTitle = "Updated-Title";
            // var managerId = "c14b2f4d-a8f0-4c35-b8d6-5c657cdc76b5";
            // var discipline = "Automation";
            // var skills = new HashSet<string> { "Automated teller machines", "Digital labor" };

            var allData = new List<object[]>
            {
                new object[] { url, "0999-9999-9999", title, "Automation", new HashSet<string> { "Automated teller machines", "Digital labor" } },
                new object[] { url, "1999-9999-9999", title, "Bionics", new HashSet<string>() }
            };

            return allData.Take(numTests);
        }

        public static IEnumerable<object[]> Data_GET(int numTests, string title)
        {
            var url = "/api/projects";

            var allData = new List<object[]>
            {
                new object[] { url, "0999-9999-9999", title },
                new object[] { url, "1999-9999-9999", title }
            };

            return allData.Take(numTests);
        }

        public static IEnumerable<object[]> Data_DEL_GET_FINAL(int numTests)
        {
            var url = "/api/projects";

            var allData = new List<object[]>
            {
                new object[] { url, "0999-9999-9999" },
                new object[] { url, "1999-9999-9999" }
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

        private static IEnumerable<OpeningPositionsSummary> GetOpenings(string discipline, HashSet<string> skills)
        {
            var opening = new OpeningPositionsSummary
            {
                PositionID = 0,
                CommitmentMonthlyHours = new Dictionary<string, int> { ["2020-05-01"] = 5 },
                Discipline = discipline,
                YearsOfExp = "1-3",
                Skills = skills
            };
            return Enumerable.Empty<OpeningPositionsSummary>().Append(opening);
        }

        private static ProjectProfile GetProjectProfile(string managerId, string projectNumber, string title, string discipline, HashSet<string> skills)
        {
            return new ProjectProfile
            {
                ProjectManager = GetProjectManager(managerId, "Clinton", "Barton"),
                ProjectSummary = GetProjectSummary(title, projectNumber),
                UsersSummary = Enumerable.Empty<UserSummary>(),
                Openings = GetOpenings(discipline, skills)
            };
        }
    }
}