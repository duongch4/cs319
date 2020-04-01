using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Web.API.Application.Communication;

using Serilog;

namespace Web.API.Authorization
{
    public class AuthorizationPolicyEvaluator : PolicyEvaluator
    {
        internal static string contextKey = "AuthorizationException";
        private readonly IHttpContextAccessor _httpContext;
        public AuthorizationPolicyEvaluator(IHttpContextAccessor httpContext, IAuthorizationService authorization) : base(authorization)
        { 
            _httpContext = httpContext;
        }

        public override async Task<PolicyAuthorizationResult> AuthorizeAsync(
            AuthorizationPolicy policy, AuthenticateResult authenticationResult, HttpContext context, object resource
        )
        {
            var result = await base.AuthorizeAsync(policy, authenticationResult, context, resource);

            if (result.Challenged)
            {
                context.Items[contextKey] = new UnauthorizedException(GetExceptionMessageFromHttpContext());
                return PolicyAuthorizationResult.Challenge();
            }
            else if (result.Forbidden)
            {
                // If user is authenticated but not allowed, send them to a special error page
                if (context.User.Identity.IsAuthenticated)
                {
                    context.Items[contextKey] = new ForbiddenException(GetExceptionMessageFromHttpContext());
                    return PolicyAuthorizationResult.Forbid();
                }

            }
            return result;
        }

        private string GetExceptionMessageFromHttpContext()
        {
            return _httpContext.HttpContext.Items[contextKey] as string;
        }
    }
}
