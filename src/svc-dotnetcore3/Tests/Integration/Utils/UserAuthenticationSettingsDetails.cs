namespace Tests.Integration.Utils
{
    /// <summary>
    /// Settings for authenticating a test
    /// request as a user
    /// </summary>
    public class UserAuthenticationSettingsDetails
    {
        /// <summary>
        /// Username of a user registered in AAD.
        /// Should be from a test tenant, not a production one.
        /// Cannot have MFA enabled.
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Password of the user
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Scope of the user
        /// </summary>
        public string Scope { get; set; }
    }
}