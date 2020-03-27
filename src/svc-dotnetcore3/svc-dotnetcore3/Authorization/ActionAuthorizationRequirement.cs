using Microsoft.AspNetCore.Authorization;

namespace Web.API.Authorization
{
    internal class ActionAuthorizationRequirement : IAuthorizationRequirement
    {
        public ActionAuthorizationRequirement(string action)
        {
            Action = action;
        }

        public string Action { get; }
    }
}
