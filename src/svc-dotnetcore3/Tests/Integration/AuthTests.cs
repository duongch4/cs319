using Tests.Integration.Utils;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using Xunit;

using Serilog;

namespace Tests.Integration
{
    public class AuthTests : IntegrationTestBase
    {
        public AuthTests(AppFixture app) : base(app)
        { }

        [Theory]
        [InlineData("/api/locations")]
        [InlineData("/api/masterlists")]
        [InlineData("/api/projects")]
        [InlineData("/api/users")]
        public async Task Request_NoAuthentication_Unauthorized(string url)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            var res = await Client.SendAsync(req);
            Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
        }

        [Theory]
        [InlineData("/api/locations")]
        [InlineData("/api/masterlists")]
        [InlineData("/api/projects")]
        [InlineData("/api/users")]
        public async Task Request_Auth_App_Pass(string url)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            await AccessTokenProvider.AuthenticateRequestAsAppAsync(req);
            var res = await Client.SendAsync(req);
            // Assert.Equal(HttpStatusCode.OK, res.StatusCode);

            // res.EnsureSuccessStatusCode(); // Status Code: 200-299            
            // var responseString = await res.Content.ReadAsStringAsync();
            // var responseJSON = JsonConvert.DeserializeObject<OkResponse<IEnumerable<ProjectSummary>>>(responseString);
        }

        [Theory]
        [InlineData("/api/locations")]
        [InlineData("/api/masterlists")]
        [InlineData("/api/projects")]
        [InlineData("/api/users")]
        public async Task Request_Auth_UserAdmin_Pass(string url)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            await AccessTokenProvider.AuthenticateRequestAsUserAsync(req, Settings.UserAdmin);
            var res = await Client.SendAsync(req);
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        }

        [Theory]
        [InlineData("/api/locations")]
        [InlineData("/api/users")]
        [InlineData("/api/projects")]
        public async Task Request_Auth_UserRegular_Pass(string url)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            await AccessTokenProvider.AuthenticateRequestAsUserAsync(req, Settings.UserRegular);
            var res = await Client.SendAsync(req);
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        }

        [Theory]
        [InlineData("/api/masterlists")]
        public async Task Request_Auth_UserRegular_Forbidden(string url)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            await AccessTokenProvider.AuthenticateRequestAsUserAsync(req, Settings.UserRegular);
            var res = await Client.SendAsync(req);
            Assert.Equal(HttpStatusCode.Forbidden, res.StatusCode);
        }

        [Theory]
        [InlineData("/api/locations")]
        [InlineData("/api/masterlists")]
        [InlineData("/api/projects")]
        [InlineData("/api/users")]
        public async Task Request_Auth_UserNotInDatabase_Forbidden(string url)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            await AccessTokenProvider.AuthenticateRequestAsUserAsync(req, Settings.UserNotInDatabase);
            var res = await Client.SendAsync(req);
            Assert.Equal(HttpStatusCode.Forbidden, res.StatusCode);
        }

        [Theory]
        [InlineData("/api/locations")]
        [InlineData("/api/masterlists")]
        [InlineData("/api/projects")]
        [InlineData("/api/users")]
        public async Task Request_Auth_UserNoRoles_CannotSignedIn(string url)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            await AccessTokenProvider.AuthenticateRequestAsUserAsync(req, Settings.UserNoRoles);
            Assert.True(string.IsNullOrEmpty(req.Headers.Authorization.Parameter));
            var res = await Client.SendAsync(req);
            Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
        }
    }

    public class AuthTestsUserAdminScopeAdminOnly : IntegrationTestBase
    {
        public AuthTestsUserAdminScopeAdminOnly(AppFixture app) : base(app)
        { }

        [Theory]
        [InlineData("/api/locations")]
        [InlineData("/api/masterlists")]
        [InlineData("/api/projects")]
        [InlineData("/api/users")]
        public async Task Request_Auth_UserAdmin_ScopeAdminOnly_Forbidden(string url)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            var settings = Settings.UserAdmin;
            settings.Scope = Settings.WrongRolesScopesComb.Admin_ScopeAdminOnly;
            await AccessTokenProvider.AuthenticateRequestAsUserAsync(req, settings);
            var res = await Client.SendAsync(req);
            Assert.Equal(HttpStatusCode.Forbidden, res.StatusCode);
        }
    }

    public class AuthTestsUserAdminScopeRegularOnly : IntegrationTestBase
    {
        public AuthTestsUserAdminScopeRegularOnly(AppFixture app) : base(app)
        { }

        [Theory]
        [InlineData("/api/locations")]
        [InlineData("/api/masterlists")]
        [InlineData("/api/projects")]
        [InlineData("/api/users")]
        public async Task Request_Auth_UserAdmin_ScopeRegularOnly_Forbidden(string url)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            var settings = Settings.UserAdmin;
            settings.Scope = Settings.WrongRolesScopesComb.Admin_ScopeRegularOnly;
            await AccessTokenProvider.AuthenticateRequestAsUserAsync(req, settings);
            var res = await Client.SendAsync(req);
            Assert.Equal(HttpStatusCode.Forbidden, res.StatusCode);
        }
    }

    public class AuthTestsUserRegularScopeAdminOnly : IntegrationTestBase
    {
        public AuthTestsUserRegularScopeAdminOnly(AppFixture app) : base(app)
        { }

        [Theory]
        [InlineData("/api/locations")]
        [InlineData("/api/masterlists")]
        [InlineData("/api/projects")]
        [InlineData("/api/users")]
        public async Task Request_Auth_UserRegular_ScopeAdminOnly_Forbidden(string url)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            var settings = Settings.UserRegular;
            settings.Scope = Settings.WrongRolesScopesComb.Regular_ScopeAdminOnly;
            await AccessTokenProvider.AuthenticateRequestAsUserAsync(req, settings);
            var res = await Client.SendAsync(req);
            Assert.Equal(HttpStatusCode.Forbidden, res.StatusCode);
        }
    }

    public class AuthTestsUserRegularScopeBoth : IntegrationTestBase
    {
        public AuthTestsUserRegularScopeBoth(AppFixture app) : base(app)
        { }

        [Theory]
        [InlineData("/api/locations")]
        [InlineData("/api/masterlists")]
        [InlineData("/api/projects")]
        [InlineData("/api/users")]
        public async Task Request_Auth_UserRegular_ScopeBoth_Forbidden(string url)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            var settings = Settings.UserRegular;
            settings.Scope = Settings.WrongRolesScopesComb.Regular_ScopeBoth;
            await AccessTokenProvider.AuthenticateRequestAsUserAsync(req, settings);
            var res = await Client.SendAsync(req);
            Assert.Equal(HttpStatusCode.Forbidden, res.StatusCode);
        }
    }
}
