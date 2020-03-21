using System.Collections.Generic;

namespace Web.API.Authorization
{
    internal static class AuthorizedPermissions
    {
        /// <summary>
        /// Contains the allowed delegated permissions for each action.
        /// If the caller has one of the allowed ones, they should be allowed
        /// to perform the action.
        /// </summary>
        public static IReadOnlyDictionary<string, string[]> DelegatedPermissionsForActions = new Dictionary<string, string[]>
        {
            [Actions.AdminThings] = new[] { DelegatedPermissions.AdminThings },
            [Actions.RegularThings] = new[] { DelegatedPermissions.RegularThings }
        };

        /// <summary>
        /// Contains the allowed application permissions for each action.
        /// If the caller has one of the allowed ones, they should be allowed
        /// to perform the action.
        /// </summary>
        public static IReadOnlyDictionary<string, string[]> ApplicationPermissionsForActions = new Dictionary<string, string[]>
        {
            [Actions.AdminThings] = new[] { ApplicationPermissions.DoAllAdminThings },
            [Actions.RegularThings] = new[] { ApplicationPermissions.DoAllRegularThings }
        };
    }
}
