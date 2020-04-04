using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

namespace Web.API.Authorization
{
    internal class ActionAuthorizationRequirementHandler : AuthorizationHandler<ActionAuthorizationRequirement>
    {
        private readonly IHttpContextAccessor _httpContext;
        public ActionAuthorizationRequirementHandler(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ActionAuthorizationRequirement requirement)
        {
            // Checks the user has a permission accepted for this action
            string[] delegatedPermissions = context.User.FindAll(Claims.ScopeClaimType).Select(c => c.Value).ToArray();
            string[] acceptedDelegatedPermissions = AuthorizedPermissions.DelegatedPermissionsForActions[requirement.Action];
            string[] appPermissionsOrRoles = context.User.FindAll(Claims.AppPermissionOrRolesClaimType).Select(c => c.Value).ToArray();
            string[] acceptedApplicationPermissions = AuthorizedPermissions.ApplicationPermissionsForActions[requirement.Action];

            if (acceptedDelegatedPermissions.Any(accepted => delegatedPermissions.Contains(accepted)))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
            else if (acceptedApplicationPermissions.Any(accepted => appPermissionsOrRoles.Contains(accepted)))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            if (_httpContext.HttpContext.Items.ContainsKey(AuthorizationPolicyEvaluator.contextKey))
            {
                var prevMes = _httpContext.HttpContext.Items[AuthorizationPolicyEvaluator.contextKey] as string;
                _httpContext.HttpContext.Items[AuthorizationPolicyEvaluator.contextKey] = $@"{prevMes} Failed ActionAuthorizationRequirement!".Trim();
            }
            return Task.CompletedTask;
        }
    }
}
