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
            var res = await DeleteResource($"{url}/{id}/skills/{name}");
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        }

        [Theory, TestPriority(4)]
        [MemberData(nameof(Data_POST_Pass))]
        public async Task Cleanup_Discipline_0(string url, string disciplineName)
        {
            var id = await GetDisciplineIdFromName(disciplineName);
            var res = await DeleteResource($"{url}/{id}");
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
            var res = await DeleteResource($"{url}/{id}");
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
            var res = await DeleteResource($"{url}/{id}");
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

        private async Task<HttpResponseMessage> DeleteResource(string url)
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
            var allData = new List<object[]> // Using these names for both disciplines and skills
            {
                new object[] { url, "D" },
                new object[] { url, "Disc" },
                new object[] { url, "12345Disc" },
                new object[] { url, "Disc12345" },
                new object[] { url, "Disc12345Skill" },
                new object[] { url, "12345Skill54321" },
                new object[] { url, "12345-Skill-54321" },
                new object[] { url, "Disc_12345_Skill" },
                new object[] { url, "Disc_12345-Skill" }
            };
            return allData;
        }

        public static IEnumerable<object[]> Data_POST_SpecialChars_Pass()
        {
            var url = "/api/admin/disciplines";
            var allData = new List<object[]>
            {
                new object[] { url, "!@#$%^&*()_+-=[]{}\\|;:'\",./<>?" } // Will NOT WORK for URL-Encoded param for skill name!!
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

    [TestCaseOrderer("Tests.Integration.Utils.PriorityOrderer", "Tests")]
    public class AdminControllerLocationTests : IntegrationTestBase
    {
        private static readonly string DOES_NOT_MATTER = "DOES NOT MATTER";
        public AdminControllerLocationTests(AppFixture app) : base(app)
        { }

        [Theory, TestPriority(0)]
        [MemberData(nameof(Data), parameters: "/api/admin/provinces")]
        public async Task CreateOneProvince_Pass(string url, string province)
        {
            var res = await GetResponseMessage_Location_POST(url, province, DOES_NOT_MATTER);
            Assert.Equal(HttpStatusCode.Created, res.StatusCode);
        }

        [Theory, TestPriority(1)]
        [InlineData("/api/masterlists")]
        public async Task CheckProvinceCreation(string url)
        {
            var added = Data("/api/admin/provinces").Select(p => p.GetValue(1)).ToHashSet();
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            await AccessTokenProvider.AuthenticateRequestAsAppAsync(req);
            var res = await Client.SendAsync(req);
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            string jsonString = await res.Content.ReadAsStringAsync();
            var jsonObject = JsonConvert.DeserializeObject<OkResponse<MasterResource>>(jsonString);
            Assert.True(added.IsProperSubsetOf(jsonObject.payload.Locations.Keys));
        }

        [Theory, TestPriority(2)]
        [MemberData(nameof(Data), parameters: "/api/admin/locations")]
        public async Task CreateOneCityOfProvince_Pass(string url, string name)
        {
            var res = await GetResponseMessage_Location_POST(url, name, name);
            Assert.Equal(HttpStatusCode.Created, res.StatusCode);
        }

        [Theory, TestPriority(3)]
        [MemberData(nameof(Data), parameters: "/api/admin/locations")]
        public async Task Cleanup_Skill(string url, string name)
        {
            var id = await GetLocationId(name, name);
            var res = await DeleteResource($"{url}/{id}");
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        }

        [Theory, TestPriority(4)]
        [MemberData(nameof(Data), parameters: "/api/admin/provinces")]
        public async Task Cleanup_Province_0(string url, string province)
        {
            var res = await DeleteResource($"{url}/{province}");
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        }

        [Theory, TestPriority(5)]
        [MemberData(nameof(Data), parameters: "/api/admin/provinces")]
        public async Task CreateProvinces_SameName_Fail(string url, string province)
        {
            var res_1 = await GetResponseMessage_Location_POST(url, province, DOES_NOT_MATTER);
            Assert.Equal(HttpStatusCode.Created, res_1.StatusCode);

            var res_2 = await GetResponseMessage_Location_POST(url, province, DOES_NOT_MATTER);
            Assert.Equal(HttpStatusCode.InternalServerError, res_2.StatusCode);
        }

        [Theory, TestPriority(6)]
        [MemberData(nameof(Data), parameters: "/api/admin/provinces")]
        public async Task Cleanup_Province_1(string url, string province)
        {
            var res = await DeleteResource($"{url}/{province}");
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        }

        [Theory, TestPriority(7)]
        [MemberData(nameof(Data_POST_Fail), parameters: "/api/admin/provinces")]
        public async Task CreateOneProvince_Fail(string url, string province)
        {
            var res = await GetResponseMessage_Location_POST(url, province, DOES_NOT_MATTER);
            Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
        }

        private async Task<HttpResponseMessage> GetResponseMessage_Location_POST(string url, string province, string city)
        {
            var location = JsonConvert.SerializeObject(GetLocationResource(province, city));
            var req = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(
                    location,
                    Encoding.UTF8,
                    "application/json"
                )
            };
            await AccessTokenProvider.AuthenticateRequestAsAppAsync(req);
            return await Client.SendAsync(req);
        }

        private async Task<int> GetLocationId(string province, string city)
        {
            var reqMasterlist = new HttpRequestMessage(HttpMethod.Get, "/api/masterlists");
            await AccessTokenProvider.AuthenticateRequestAsAppAsync(reqMasterlist);
            var resMasterlist = await Client.SendAsync(reqMasterlist);
            string jsonString = await resMasterlist.Content.ReadAsStringAsync();
            var locations = JsonConvert.DeserializeObject<OkResponse<MasterResource>>(jsonString).payload.Locations;
            return locations[province][city];
        }

        private async Task<HttpResponseMessage> DeleteResource(string url)
        {
            var req = new HttpRequestMessage(HttpMethod.Delete, url);
            await AccessTokenProvider.AuthenticateRequestAsAppAsync(req);
            return await Client.SendAsync(req);
        }

        private LocationResource GetLocationResource(string province, string city)
        {
            return new LocationResource
            {
                LocationID = 0, // DOES NOT MATTER
                Province = province,
                City = city
            };
        }

        public static IEnumerable<object[]> Data(string url)
        {
            // var url = "/api/admin/locations";
            var allData = new List<object[]> // Using these names for both provinces and cities
            {
                new object[] { url, "D" },
                new object[] { url, "Disc" },
                new object[] { url, "12345Disc" },
                new object[] { url, "Disc12345" },
                new object[] { url, "Disc12345Skill" },
                new object[] { url, "12345Skill54321" },
                new object[] { url, "12345-Skill-54321" },
                new object[] { url, "Disc_12345_Skill" },
                new object[] { url, "Disc_12345-Skill" }
            };
            return allData;
        }

        // public static IEnumerable<object[]> Data_POST_SpecialChars_Pass()
        // {
        //     var url = "/api/admin/disciplines";
        //     var allData = new List<object[]>
        //     {
        //         new object[] { url, "!@#$%^&*()_+-=[]{}\\|;:'\",./<>?" } // Will NOT WORK for URL-Encoded param for skill name!!
        //     };
        //     return allData;
        // }

        public static IEnumerable<object[]> Data_POST_Fail(string url)
        {
            var allData = new List<object[]>
            {
                new object[] { url, null },
                new object[] { url, "" }
            };
            return allData;
        }
    }


}