using System.Security.Claims;

namespace Web.API.Authorization
{
    internal static class Claims
    {
        internal const string ScopeClaimType = "http://schemas.microsoft.com/identity/claims/scope";
        internal const string ObjectIdentifier = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        internal const string AppPermissionOrRolesClaimType = ClaimTypes.Role;
    }
}
