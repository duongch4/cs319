using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Web.API.Application.Communication;

using System.Threading.Tasks;
using System.Net;

using Serilog;

namespace Web.API.Authorization
{
    public class AuthorizationAuthenticationService : AuthenticationService
    {
        public AuthorizationAuthenticationService(
            IAuthenticationSchemeProvider schemes, IAuthenticationHandlerProvider handlers,
            IClaimsTransformation transform, Microsoft.Extensions.Options.IOptions<AuthenticationOptions> options
        ) : base(schemes, handlers, transform, options)
        { }

        public override Task ChallengeAsync(HttpContext context, string scheme, AuthenticationProperties properties)
        {
            if (context.Items.ContainsKey(AuthorizationPolicyEvaluator.contextKey))
            {
                var options = context.Items[AuthorizationPolicyEvaluator.contextKey] as UnauthorizedException;
                var routeData = context.GetRouteData();
                var actionDescriptor = new ActionDescriptor();
                var actionContext = new ActionContext(context, routeData, actionDescriptor);
                var actionResult = new JsonResult(options) { StatusCode = (int)HttpStatusCode.Unauthorized };
                return actionResult.ExecuteResultAsync(actionContext);
            }
            return base.ChallengeAsync(context, scheme, properties);
        }
        public override Task ForbidAsync(HttpContext context, string scheme, AuthenticationProperties properties)
        {
            if (context.Items.ContainsKey(AuthorizationPolicyEvaluator.contextKey))
            {
                var options = context.Items[AuthorizationPolicyEvaluator.contextKey];
                var routeData = context.GetRouteData();
                var actionDescriptor = new ActionDescriptor();
                var actionContext = new ActionContext(context, routeData, actionDescriptor);
                var actionResult = new JsonResult(options) { StatusCode = (int)HttpStatusCode.Forbidden };
                return actionResult.ExecuteResultAsync(actionContext);
            }
            return base.ForbidAsync(context, scheme, properties);
        }
    }
}