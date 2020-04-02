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
    public class UsersControllerIntegrationTests : IntegrationTestBase
    {
        private static readonly string userId = "1";
        public UsersControllerIntegrationTests(AppFixture app) : base(app)
        { }

        [Theory, TestPriority(0)]
        [InlineData("/api/users")]
        public async Task UpdateOneUser_First(string url)
        {
            var userProfile = JsonConvert.SerializeObject(GetUserProfile(getTwo: true));
            var req = new HttpRequestMessage(HttpMethod.Put, $"{url}/{userId}")
            {
                Content = new StringContent(
                    userProfile,
                    Encoding.UTF8,
                    "application/json"
                )
            };
            await AccessTokenProvider.AuthenticateRequestAsAppAsync(req);
            var res = await Client.SendAsync(req);
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        }

        [Theory, TestPriority(1)]
        [InlineData("/api/users")]
        public async Task GetOneUser_ExpectTwoDisciplines(string url)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, $"{url}/{userId}");
            await AccessTokenProvider.AuthenticateRequestAsAppAsync(req);
            var res = await Client.SendAsync(req);
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            string jsonString = await res.Content.ReadAsStringAsync();
            var jsonObject = JsonConvert.DeserializeObject<OkResponse<UserProfile>>(jsonString);
            Assert.Equal(2, jsonObject.payload.Disciplines.Count());
        }

        [Theory, TestPriority(2)]
        [InlineData("/api/users")]
        public async Task UpdateOneUser_Second(string url)
        {
            var userProfile = JsonConvert.SerializeObject(GetUserProfile(getTwo: false));
            var req = new HttpRequestMessage(HttpMethod.Put, $"{url}/{userId}")
            {
                Content = new StringContent(
                    userProfile,
                    Encoding.UTF8,
                    "application/json"
                )
            };
            await AccessTokenProvider.AuthenticateRequestAsAppAsync(req);
            var res = await Client.SendAsync(req);
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        }

        [Theory, TestPriority(3)]
        [InlineData("/api/users")]
        public async Task GetOneUser_ExpectOneDiscipline(string url)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, $"{url}/{userId}");
            await AccessTokenProvider.AuthenticateRequestAsAppAsync(req);
            var res = await Client.SendAsync(req);
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            string jsonString = await res.Content.ReadAsStringAsync();
            var jsonObject = JsonConvert.DeserializeObject<OkResponse<UserProfile>>(jsonString);
            Assert.Single(jsonObject.payload.Disciplines);
        }

        [Theory]
        [InlineData("/api/users/search")]
        public async Task SearchUsers(string url)
        {
            var searchWord = "ge";
            var reqSearch = JsonConvert.SerializeObject(GetRequestSearch(searchWord));
            var req = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(
                    reqSearch,
                    Encoding.UTF8,
                    "application/json"
                )
            };
            await AccessTokenProvider.AuthenticateRequestAsAppAsync(req);
            var res = await Client.SendAsync(req);
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            string jsonString = await res.Content.ReadAsStringAsync();
            var jsonObject = JsonConvert.DeserializeObject<OkResponse<IEnumerable<UserSummary>>>(jsonString);
            var userNames = jsonObject.payload.Select(us => $"{us.FirstName} {us.LastName}").ToList();
            Assert.True(userNames.All(name => name.ToLower().Trim().Contains(searchWord)));
        }

        private static RequestSearchUsers GetRequestSearch(string searchWord)
        {
            return new RequestSearchUsers
            {
                SearchWord = searchWord,
                Filter = null,
                OrderKey = null,
                Order = null,
                Page = 1
            };
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

        private static UserSummary GetUserSummary()
        {
            return new UserSummary
            {
                UserID = userId,
                FirstName = "Lucia",
                LastName = "Stein",
                Location = new LocationResource
                {
                    LocationID = 9,
                    Province = "Alberta",
                    City = "Fort McMurray"
                },
                Utilization = 0,
                ResourceDiscipline = null,
                IsConfirmed = false
            };
        }

        // private static IEnumerable<ProjectSummary> GetCurrentProjects()
        // {
        //     var project = new ProjectSummary
        //     {
        //         Title = "Third Pointless Pottery",
        //         ProjectNumber = "1B8G20PITVRUMHS",
        //         Location = new LocationResource
        //         {
        //             LocationID = 20,
        //             Province = "Ontario",
        //             City = "Kitchener"
        //         },
        //         ProjectStartDate = new DateTime(2019, 7, 16),
        //         ProjectEndDate = new DateTime(2029, 11, 18)
        //     };
        //     return Enumerable.Empty<ProjectSummary>().Append(project);
        // }

        private static IEnumerable<OutOfOfficeResource> GetAvailability()
        {
            var availability = new OutOfOfficeResource
            {
                FromDate = new DateTime(2020, 12, 15),
                ToDate = new DateTime(2021, 1, 10),
                Reason = "Vacation"
            };
            return Enumerable.Empty<OutOfOfficeResource>().Append(availability);
        }

        private static ResourceDisciplineResource GetDisciplineResource(string discipeline, HashSet<string> skills, string yearsOfExp)
        {
            return new ResourceDisciplineResource
            {
                DisciplineID = 32,
                Discipline = discipeline,
                Skills = skills,
                YearsOfExp = yearsOfExp
            };
        }

        private static IEnumerable<ResourceDisciplineResource> GetDisciplines_OneElem()
        {
            var skills_one = new HashSet<string> { "Aircraft manufacturing?", "Automotive engineering?" };
            var discipline_one = GetDisciplineResource("Mechanical engineering", skills_one, "1-3");
            return Enumerable.Empty<ResourceDisciplineResource>().Append(discipline_one);
        }

        private static IEnumerable<ResourceDisciplineResource> GetDisciplines_TwoElems()
        {
            var skills_one = new HashSet<string> { "Aircraft manufacturing?", "Automotive engineering?" };
            var discipline_one = GetDisciplineResource("Mechanical engineering", skills_one, "1-3");
            var skills_two = new HashSet<string>();
            var discipline_two = GetDisciplineResource("Design Engineering", skills_two, "10+");
            return Enumerable.Empty<ResourceDisciplineResource>().Append(discipline_one).Append(discipline_two);
        }

        // private static IEnumerable<PositionSummary> GetPositions()
        // {
        //     var hours = new Dictionary<string, int>
        //     {
        //         ["2023-01-19"] = 85,
        //         ["2023-02-27"] = 177,
        //         ["2023-03-18"] = 94,
        //         ["2023-04-22"] = 26,
        //         ["2023-05-05"] = 49,
        //         ["2023-06-17"] = 60,
        //         ["2023-07-17"] = 38,
        //         ["2023-08-06"] = 101,
        //         ["2023-09-17"] = 128,
        //         ["2023-10-06"] = 116,
        //         ["2023-11-14"] = 155,
        //         ["2023-12-03"] = 134,
        //     };
        //     var discipline = new PositionSummary
        //     {
        //         DisciplineName = "Structural engineering",
        //         PositionID = 1399,
        //         PositionName = "Name Me: DOES NOT MATTER: WHY AM I HERE??? ",
        //         ProjectTitle = "hird Pointless Pottery",
        //         ProjectedMonthlyHours = hours
        //     };
        //     return Enumerable.Empty<PositionSummary>().Append(discipline);
        // }

        private static UserProfile GetUserProfile(bool getTwo = true)
        {
            return new UserProfile
            {
                UserSummary = GetUserSummary(),
                Availability = GetAvailability(),
                Disciplines = getTwo ? GetDisciplines_TwoElems() : GetDisciplines_OneElem(),
                CurrentProjects = null,
                Positions = null
            };
        }
    }
}