using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Web.API.Application.Repository;
using Web.API.Application.Models;

namespace Web.API.Authorization
{
    internal class AnyValidPermissionRequirementHandler : AuthorizationHandler<AnyValidPermissionRequirement>
    {
        private readonly IUsersRepository _usersRepository;
        public AnyValidPermissionRequirementHandler(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AnyValidPermissionRequirement requirement)
        {
            string objectId = context.User.FindAll(Claims.ObjectIdentifier).Select(c => c.Value).FirstOrDefault();
            if (!String.IsNullOrEmpty(objectId))
            {
                // Checks caller has at least one valid permission
                string[] delegatedPermissions = context.User.FindAll(Claims.ScopeClaimType).Select(c => c.Value).ToArray();
                string[] allAcceptedDelegatedPermissions = DelegatedPermissions.All;
                string[] appPermissionsOrRoles = context.User.FindAll(Claims.AppPermissionOrRolesClaimType).Select(c => c.Value).ToArray();
                string[] allAcceptedApplicationPermissions = ApplicationPermissions.All;
                if (delegatedPermissions.Any(p => allAcceptedDelegatedPermissions.Contains(p)))
                {
                    // Caller has a valid delegated permission
                    // If your API has different user roles,
                    // this is where you would check that, before calling context.Succeed()
                    if (appPermissionsOrRoles.Any(p => allAcceptedApplicationPermissions.Contains(p)))
                    {
                        if (await IsAuthorized(appPermissionsOrRoles, delegatedPermissions, objectId))
                        {
                            context.Succeed(requirement);
                            return;
                        }
                    }
                }
                else if (appPermissionsOrRoles.Any(p => allAcceptedApplicationPermissions.Contains(p)))
                {
                    // Caller has a valid application permission
                    context.Succeed(requirement);
                    return;
                }

                // If we reached here without calling context.Succeed(),
                // the call will fail with a 403 Forbidden
                context.Fail();
            }

        }

        private bool IsRoleAdmin(string[] appPermissionsOrRoles)
        {
            return appPermissionsOrRoles.Contains("adminUser");
        }

        private bool IsScopeAdmin(string[] delegatedPermissions)
        {
            return (
                (delegatedPermissions.Count() > 1) &&
                delegatedPermissions.Contains("Admin")
            );
        }

        private bool IsScopeRegular(string[] delegatedPermissions)
        {
            return (
                (delegatedPermissions.Count() == 1) &&
                delegatedPermissions.Contains("Regular")
            );
        }

        private bool IsObjectIdMatchedDatabaseAdmin(User user)
        {
            return (user != null && user.IsAdmin);
        }

        private bool IsObjectIdMatchedDatabaseRegular(User user)
        {
            return (user != null && !user.IsAdmin);
        }

        private async Task<bool> IsAuthorized(string[] appPermissionsOrRoles, string[] delegatedPermissions, string objectId)
        {
            bool isRoleAdmin = IsRoleAdmin(appPermissionsOrRoles);
            var user = await _usersRepository.GetAUser(objectId);
            return (
                (isRoleAdmin && IsScopeAdmin(delegatedPermissions) && IsObjectIdMatchedDatabaseAdmin(user)) ||   // Either an Admin
                (!isRoleAdmin && IsScopeRegular(delegatedPermissions) && IsObjectIdMatchedDatabaseRegular(user)) // OR a Regular
            );
        }
    }
}
