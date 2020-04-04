namespace Tests.Integration.Utils
{
    /// <summary>
    /// Settings for authenticating a test
    /// request as a user
    /// </summary>
    public class UserAuthenticationSettingsWrongRolesScopesComb
    {
        /// <summary>
        /// Role: Admin
        /// Scope: Regular Only
        /// </summary>
        public string Admin_ScopeRegularOnly { get; set; }
        /// <summary>
        /// Role: Admin
        /// Scope: Admin Only
        /// </summary>
        public string Admin_ScopeAdminOnly { get; set; }
        /// <summary>
        /// Role: Regular
        /// Scope: Admin Only
        /// </summary>
        public string Regular_ScopeAdminOnly { get; set; }
        /// <summary>
        /// Role: Regular
        /// Scope: Both Admin and Regular
        /// </summary>
        public string Regular_ScopeBoth { get; set; }
    }
}