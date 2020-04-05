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
    public class AdminControllerDisciplineAndSkillTests : IntegrationTestBase
    {
        public AdminControllerDisciplineAndSkillTests(AppFixture app) : base(app)
        { }

        [Theory, TestPriority(0)]
        [MemberData(nameof(Data_POST_Pass))]
        public async Task CreateOneDiscipline_Pass(string url, string disciplineName)
        {
            var res = await GetResponseMessage_Discipline_POST(url, disciplineName);
            Assert.Equal(HttpStatusCode.Created, res.StatusCode);
        }

        [Theory, TestPriority(1)]
        [InlineData("/api/masterlists")]
        public async Task GetDisciplines_AfterCreate(string url)
        {
            var added = Data_POST_Pass().Select(p => p.GetValue(1)).ToHashSet();
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            await AccessTokenProvider.AuthenticateRequestAsAppAsync(req);
            var res = await Client.SendAsync(req);
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            string jsonString = await res.Content.ReadAsStringAsync();
            var jsonObject = JsonConvert.DeserializeObject<OkResponse<MasterResource>>(jsonString);
            Assert.True(added.IsProperSubsetOf(jsonObject.payload.Disciplines.Keys));
        }

        [Theory, TestPriority(2)]
        [MemberData(nameof(Data_POST_Pass))]
        public async Task CreateOneSkill_Pass(string url, string name)
        {
            var id = await GetDisciplineIdFromName(name);
            var res = await GetResponseMessage_Skill_POST($"{url}/{id}/skills", id, name);
            Assert.Equal(HttpStatusCode.Created, res.StatusCode);
        }

        [Theory, TestPriority(3)]
        [MemberData(nameof(Data_POST_Pass))]
        public async Task Cleanup_Skill(string url, string name)
        {
            var id = await GetDisciplineIdFromName(name);
            var res = await DeleteDisciplineOrSkill($"{url}/{id}/skills/{name}");
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        }

        [Theory, TestPriority(4)]
        [MemberData(nameof(Data_POST_Pass))]
        public async Task Cleanup_Discipline_0(string url, string disciplineName)
        {
            var id = await GetDisciplineIdFromName(disciplineName);
            var res = await DeleteDisciplineOrSkill($"{url}/{id}");
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        }

        [Theory, TestPriority(5)]
        [MemberData(nameof(Data_POST_SpecialChars_Pass))]
        public async Task CreateOneDiscipline_SpecialChars_Pass(string url, string disciplineName)
        {
            var res = await GetResponseMessage_Discipline_POST(url, disciplineName);
            Assert.Equal(HttpStatusCode.Created, res.StatusCode);
        }

        [Theory, TestPriority(6)]
        [MemberData(nameof(Data_POST_SpecialChars_Pass))]
        public async Task Cleanup_Discipline_1(string url, string disciplineName)
        {
            var id = await GetDisciplineIdFromName(disciplineName);
            var res = await DeleteDisciplineOrSkill($"{url}/{id}");
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        }

        [Theory, TestPriority(7)]
        [InlineData("/api/admin/disciplines")]
        public async Task CreateDisciplines_SameName_Fail(string url)
        {
            var res_1 = await GetResponseMessage_Discipline_POST(url, "D");
            Assert.Equal(HttpStatusCode.Created, res_1.StatusCode);

            var res_2 = await GetResponseMessage_Discipline_POST(url, "D");
            Assert.Equal(HttpStatusCode.InternalServerError, res_2.StatusCode);
        }

        [Theory, TestPriority(8)]
        [InlineData("/api/admin/disciplines")]
        public async Task Cleanup_1(string url)
        {
            var id = await GetDisciplineIdFromName("D");
            var res = await DeleteDisciplineOrSkill($"{url}/{id}");
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        }

        [Theory, TestPriority(9)]
        [MemberData(nameof(Data_POST_Fail))]
        public async Task CreateDisciplines_Fail(string url, string disciplineName)
        {
            var res = await GetResponseMessage_Discipline_POST(url, disciplineName);
            Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
        }

        private async Task<HttpResponseMessage> GetResponseMessage_Discipline_POST(string url, string disciplineName)
        {
            var discipline = JsonConvert.SerializeObject(GetDisciplineResource(disciplineName));
            var req = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(
                    discipline,
                    Encoding.UTF8,
                    "application/json"
                )
            };
            await AccessTokenProvider.AuthenticateRequestAsAppAsync(req);
            return await Client.SendAsync(req);
        }

        private async Task<HttpResponseMessage> GetResponseMessage_Skill_POST(string url, int disciplineId, string skillName)
        {
            var discSkill = JsonConvert.SerializeObject(GetDisciplineSkillResource(disciplineId, skillName));
            var req = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(
                    discSkill,
                    Encoding.UTF8,
                    "application/json"
                )
            };
            await AccessTokenProvider.AuthenticateRequestAsAppAsync(req);
            return await Client.SendAsync(req);
        }

        private async Task<int> GetDisciplineIdFromName(string disciplineName)
        {
            var reqMasterlist = new HttpRequestMessage(HttpMethod.Get, "/api/masterlists");
            await AccessTokenProvider.AuthenticateRequestAsAppAsync(reqMasterlist);
            var resMasterlist = await Client.SendAsync(reqMasterlist);
            string jsonString = await resMasterlist.Content.ReadAsStringAsync();
            var disc = JsonConvert.DeserializeObject<OkResponse<MasterResource>>(jsonString).payload.Disciplines;
            return disc[disciplineName].DisciplineID;
        }

        private async Task<HttpResponseMessage> DeleteDisciplineOrSkill(string url)
        {
            var req = new HttpRequestMessage(HttpMethod.Delete, url);
            await AccessTokenProvider.AuthenticateRequestAsAppAsync(req);
            return await Client.SendAsync(req);
        }

        private DisciplineResource GetDisciplineResource(string disciplineName)
        {
            return new DisciplineResource
            {
                Id = 0, // DOES NOT MATTER
                Name = disciplineName,
                Skills = "DOES NOT MATTER"
            };
        }

        private DisciplineSkillResource GetDisciplineSkillResource(int disciplineId, string skillName)
        {
            return new DisciplineSkillResource
            {
                DisciplineId = disciplineId,
                SkillId = 0, // DOES NOT MATTER
                Name = skillName
            };
        }

        public static IEnumerable<object[]> Data_POST_Pass()
        {
            var url = "/api/admin/disciplines";
            var allData = new List<object[]>
            {
                new object[] { url, "D" },
                new object[] { url, "Disc" }
            };
            return allData;
        }

        public static IEnumerable<object[]> Data_POST_SpecialChars_Pass()
        {
            var url = "/api/admin/disciplines";
            var allData = new List<object[]>
            {
                new object[] { url, "!@#$%^&*()_+-=[]{}\\|;:'\",./<>?" }
            };
            return allData;
        }

        public static IEnumerable<object[]> Data_POST_Fail()
        {
            var url = "/api/admin/disciplines";
            var allData = new List<object[]>
            {
                new object[] { url, null },
                new object[] { url, "" }
            };
            return allData;
        }
    }

    // [TestCaseOrderer("Tests.Integration.Utils.PriorityOrderer", "Tests")]
    // public class AdminControllerLocationTests : IntegrationTestBase
    // {
    //     public  AdminControllerLocationTests(AppFixture app) : base(app)
    //     { }

    //     [Theory, TestPriority(0)]
    //     [InlineData("/api/admin/locations")]
    //     public async Task CreateOneSkill(string url)
    //     {
    //         var userProfile = JsonConvert.SerializeObject(GetUserProfile(getTwo: true));
    //         var req = new HttpRequestMessage(HttpMethod.Put, $"{url}/{userId}")
    //         {
    //             Content = new StringContent(
    //                 userProfile,
    //                 Encoding.UTF8,
    //                 "application/json"
    //             )
    //         };
    //         await AccessTokenProvider.AuthenticateRequestAsAppAsync(req);
    //         var res = await Client.SendAsync(req);
    //         Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    //     }

    //     [Theory, TestPriority(1)]
    //     [InlineData("/api/admin/locations")]
    //     public async Task GetOneDiscipline_AfterCreate(string url)
    //     {
    //         var req = new HttpRequestMessage(HttpMethod.Get, $"{url}/{userId}");
    //         await AccessTokenProvider.AuthenticateRequestAsAppAsync(req);
    //         var res = await Client.SendAsync(req);
    //         Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    //         string jsonString = await res.Content.ReadAsStringAsync();
    //         var jsonObject = JsonConvert.DeserializeObject<OkResponse<UserProfile>>(jsonString);
    //         Assert.Equal(2, jsonObject.payload.Disciplines.Count());
    //     }

    //     [Theory, TestPriority(2)]
    //     [InlineData("/api/admin/locations")]
    //     public async Task DeleteOneDiscipline(string url)
    //     {
    //         var userProfile = JsonConvert.SerializeObject(GetUserProfile(getTwo: false));
    //         var req = new HttpRequestMessage(HttpMethod.Put, $"{url}/{userId}")
    //         {
    //             Content = new StringContent(
    //                 userProfile,
    //                 Encoding.UTF8,
    //                 "application/json"
    //             )
    //         };
    //         await AccessTokenProvider.AuthenticateRequestAsAppAsync(req);
    //         var res = await Client.SendAsync(req);
    //         Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    //     }

    //     [Theory, TestPriority(3)]
    //     [InlineData("/api/admin/locations")]
    //     public async Task GetOneDiscipline_AfterDelete(string url)
    //     {
    //         var req = new HttpRequestMessage(HttpMethod.Get, $"{url}/{userId}");
    //         await AccessTokenProvider.AuthenticateRequestAsAppAsync(req);
    //         var res = await Client.SendAsync(req);
    //         Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    //         string jsonString = await res.Content.ReadAsStringAsync();
    //         var jsonObject = JsonConvert.DeserializeObject<OkResponse<UserProfile>>(jsonString);
    //         Assert.Single(jsonObject.payload.Disciplines);
    //     }
    // }

    // [TestCaseOrderer("Tests.Integration.Utils.PriorityOrderer", "Tests")]
    // public class AdminControllerProvinceTests : IntegrationTestBase
    // {
    //     public  AdminControllerProvinceTests(AppFixture app) : base(app)
    //     { }

    //     [Theory, TestPriority(0)]
    //     [InlineData("/api/admin/provinces")]
    //     public async Task CreateOneSkill(string url)
    //     {
    //         var userProfile = JsonConvert.SerializeObject(GetUserProfile(getTwo: true));
    //         var req = new HttpRequestMessage(HttpMethod.Put, $"{url}/{userId}")
    //         {
    //             Content = new StringContent(
    //                 userProfile,
    //                 Encoding.UTF8,
    //                 "application/json"
    //             )
    //         };
    //         await AccessTokenProvider.AuthenticateRequestAsAppAsync(req);
    //         var res = await Client.SendAsync(req);
    //         Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    //     }

    //     [Theory, TestPriority(1)]
    //     [InlineData("/api/admin/provinces")]
    //     public async Task GetOneDiscipline_AfterCreate(string url)
    //     {
    //         var req = new HttpRequestMessage(HttpMethod.Get, $"{url}/{userId}");
    //         await AccessTokenProvider.AuthenticateRequestAsAppAsync(req);
    //         var res = await Client.SendAsync(req);
    //         Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    //         string jsonString = await res.Content.ReadAsStringAsync();
    //         var jsonObject = JsonConvert.DeserializeObject<OkResponse<UserProfile>>(jsonString);
    //         Assert.Equal(2, jsonObject.payload.Disciplines.Count());
    //     }

    //     [Theory, TestPriority(2)]
    //     [InlineData("/api/admin/provinces")]
    //     public async Task DeleteOneDiscipline(string url)
    //     {
    //         var userProfile = JsonConvert.SerializeObject(GetUserProfile(getTwo: false));
    //         var req = new HttpRequestMessage(HttpMethod.Put, $"{url}/{userId}")
    //         {
    //             Content = new StringContent(
    //                 userProfile,
    //                 Encoding.UTF8,
    //                 "application/json"
    //             )
    //         };
    //         await AccessTokenProvider.AuthenticateRequestAsAppAsync(req);
    //         var res = await Client.SendAsync(req);
    //         Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    //     }

    //     [Theory, TestPriority(3)]
    //     [InlineData("/api/admin/provinces")]
    //     public async Task GetOneDiscipline_AfterDelete(string url)
    //     {
    //         var req = new HttpRequestMessage(HttpMethod.Get, $"{url}/{userId}");
    //         await AccessTokenProvider.AuthenticateRequestAsAppAsync(req);
    //         var res = await Client.SendAsync(req);
    //         Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    //         string jsonString = await res.Content.ReadAsStringAsync();
    //         var jsonObject = JsonConvert.DeserializeObject<OkResponse<UserProfile>>(jsonString);
    //         Assert.Single(jsonObject.payload.Disciplines);
    //     }
    // }

    // private static RequestSearchUsers GetRequestSearch(string searchWord)
    // {
    //     return new RequestSearchUsers
    //     {
    //         SearchWord = searchWord,
    //         Filter = null,
    //         OrderKey = null,
    //         Order = null,
    //         Page = 1
    //     };
    // }

    // private static IEnumerable<object[]> GetData_POST(int numTests)
    // {
    //     var allData = new List<object[]>
    //     {
    //         new object[] { 1, 2, 3 },
    //         new object[] { -4, -6, -10 },
    //         new object[] { -2, 2, 0 },
    //         new object[] { int.MinValue, -1, int.MaxValue },
    //     };

    //     return allData.Take(numTests);
    // }

    // private static UserSummary GetUserSummary()
    // {
    //     return new UserSummary
    //     {
    //         UserID = userId,
    //         FirstName = "Lucia",
    //         LastName = "Stein",
    //         Location = new LocationResource
    //         {
    //             LocationID = 9,
    //             Province = "Alberta",
    //             City = "Fort McMurray"
    //         },
    //         Utilization = 0,
    //         ResourceDiscipline = null,
    //         IsConfirmed = false
    //     };
    // }

    // // private static IEnumerable<ProjectSummary> GetCurrentProjects()
    // // {
    // //     var project = new ProjectSummary
    // //     {
    // //         Title = "Third Pointless Pottery",
    // //         ProjectNumber = "1B8G20PITVRUMHS",
    // //         Location = new LocationResource
    // //         {
    // //             LocationID = 20,
    // //             Province = "Ontario",
    // //             City = "Kitchener"
    // //         },
    // //         ProjectStartDate = new DateTime(2019, 7, 16),
    // //         ProjectEndDate = new DateTime(2029, 11, 18)
    // //     };
    // //     return Enumerable.Empty<ProjectSummary>().Append(project);
    // // }

    // private static IEnumerable<OutOfOfficeResource> GetAvailability()
    // {
    //     var availability = new OutOfOfficeResource
    //     {
    //         FromDate = new DateTime(2020, 12, 15),
    //         ToDate = new DateTime(2021, 1, 10),
    //         Reason = "Vacation"
    //     };
    //     return Enumerable.Empty<OutOfOfficeResource>().Append(availability);
    // }

    // private static ResourceDisciplineResource GetDisciplineResource(string discipline, HashSet<string> skills, string yearsOfExp)
    // {
    //     return new ResourceDisciplineResource
    //     {
    //         DisciplineID = 32,
    //         Discipline = discipline,
    //         Skills = skills,
    //         YearsOfExp = yearsOfExp
    //     };
    // }

    // private static IEnumerable<ResourceDisciplineResource> GetDisciplines_OneElem()
    // {
    //     var skills_one = new HashSet<string> { "Aircraft manufacturing?", "Automotive engineering?" };
    //     var discipline_one = GetDisciplineResource("Mechanical engineering", skills_one, "1-3");
    //     return Enumerable.Empty<ResourceDisciplineResource>().Append(discipline_one);
    // }

    // private static IEnumerable<ResourceDisciplineResource> GetDisciplines_TwoElems()
    // {
    //     var skills_one = new HashSet<string> { "Aircraft manufacturing?", "Automotive engineering?" };
    //     var discipline_one = GetDisciplineResource("Mechanical engineering", skills_one, "1-3");
    //     var skills_two = new HashSet<string>();
    //     var discipline_two = GetDisciplineResource("Design Engineering", skills_two, "10+");
    //     return Enumerable.Empty<ResourceDisciplineResource>().Append(discipline_one).Append(discipline_two);
    // }

    // // private static IEnumerable<PositionSummary> GetPositions()
    // // {
    // //     var hours = new Dictionary<string, int>
    // //     {
    // //         ["2023-01-19"] = 85,
    // //         ["2023-02-27"] = 177,
    // //         ["2023-03-18"] = 94,
    // //         ["2023-04-22"] = 26,
    // //         ["2023-05-05"] = 49,
    // //         ["2023-06-17"] = 60,
    // //         ["2023-07-17"] = 38,
    // //         ["2023-08-06"] = 101,
    // //         ["2023-09-17"] = 128,
    // //         ["2023-10-06"] = 116,
    // //         ["2023-11-14"] = 155,
    // //         ["2023-12-03"] = 134,
    // //     };
    // //     var discipline = new PositionSummary
    // //     {
    // //         DisciplineName = "Structural engineering",
    // //         PositionID = 1399,
    // //         PositionName = "Name Me: DOES NOT MATTER: WHY AM I HERE??? ",
    // //         ProjectTitle = "hird Pointless Pottery",
    // //         ProjectedMonthlyHours = hours
    // //     };
    // //     return Enumerable.Empty<PositionSummary>().Append(discipline);
    // // }

    // private static UserProfile GetUserProfile(bool getTwo = true)
    // {
    //     return new UserProfile
    //     {
    //         UserSummary = GetUserSummary(),
    //         Availability = GetAvailability(),
    //         Disciplines = getTwo ? GetDisciplines_TwoElems() : GetDisciplines_OneElem(),
    //         CurrentProjects = null,
    //         Positions = null
    //     };
    // }
    // }
}