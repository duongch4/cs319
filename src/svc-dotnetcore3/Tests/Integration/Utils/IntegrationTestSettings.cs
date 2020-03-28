namespace Tests.Integration.Utils
{
    public class IntegrationTestSettings
    {
        /// <summary>
        /// The Azure AD authority, e.g. https://login.microsoftonline.com/your-aad-tenant-id/v2.0
        /// </summary>
        public string Authority { get; set; }

        /// <summary>
        /// The App ID URI for the API app registration,
        /// e.g. api://some-guid-generated-by-aad
        /// </summary>
        public string ApplicationIdUri { get; set; }
        
        public UserAuthenticationSettings UserAuthentication { get; set; }
        public UserAuthenticationSettingsDetails UserAdmin { get; set; }
        public UserAuthenticationSettingsDetails UserRegular { get; set; }
        public UserAuthenticationSettingsDetails UserNotInDatabase { get; set; }
        public UserAuthenticationSettingsDetails UserNoRoles { get; set; }
        public AppAuthenticationSettings AppAuthentication { get; set; }
    }
}