using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Serilog;
using Web.API.Application.Repository;
using Web.API.Application.Models;
using Web.API.Resources;

namespace Web.API.Authorization
{
    internal class AnyValidPermissionRequirementHandler : AuthorizationHandler<AnyValidPermissionRequirement>
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly IUsersRepository _usersRepository;
        private readonly IProjectsRepository _projectsRepository;
        private StringBuilder _stringBuilderAdmin = new StringBuilder();
        private StringBuilder _stringBuilderRegular = new StringBuilder();
        public AnyValidPermissionRequirementHandler(IHttpContextAccessor httpContext, IUsersRepository usersRepository, IProjectsRepository projectsRepository)
        {
            _httpContext = httpContext;
            _usersRepository = usersRepository;
            _projectsRepository = projectsRepository;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AnyValidPermissionRequirement requirement)
        {
            string objectId = context.User.FindAll(Claims.ObjectIdentifier).Select(c => c.Value).FirstOrDefault();
            string nameId = context.User.FindAll(Claims.NameIdentifier).Select(c => c.Value).FirstOrDefault();
            if (String.IsNullOrEmpty(objectId))
            {
                SetExceptionMessageToHttpContext("Cannot identify Object ID!");
                context.Fail();
                return;
            }

            // Checks caller has at least one valid application/role permission
            string[] appPermissionsOrRoles = context.User.FindAll(Claims.AppPermissionOrRolesClaimType).Select(c => c.Value).ToArray();
            string[] allAcceptedApplicationPermissions = ApplicationPermissions.All;
            if (!appPermissionsOrRoles.Any(p => allAcceptedApplicationPermissions.Contains(p)))
            {
                SetExceptionMessageToHttpContext("Cannot recognise any of the application or role permissions!");
                context.Fail();
                return;
            }

            if (IsRoleScopeBased(objectId, nameId))
            {
                // Checks caller has at least one valid delegated permission
                string[] delegatedPermissions = context.User.FindAll(Claims.ScopeClaimType).Select(c => c.Value).ToArray();
                string[] allAcceptedDelegatedPermissions = DelegatedPermissions.All;
                if (!delegatedPermissions.Any(p => allAcceptedDelegatedPermissions.Contains(p)))
                {
                    SetExceptionMessageToHttpContext("Cannot recognise any of the delegated (scope) permissions!");
                    context.Fail();
                    return;
                }

                await HandleAuthorized(appPermissionsOrRoles, delegatedPermissions, objectId, context, requirement);
                return;
            }
            else // Now for App Permissions only (Used in Integration for now)
            {
                // Nothing to check since we accept any of the app permissions
                context.Succeed(requirement);
                return;
            }
        }

        private async Task<T> GetRequestBodyAsync<T>(HttpRequest request)
        {
            T objRequestBody = default(T);

            // IMPORTANT: Ensure the requestBody can be read multiple times.
            HttpRequestRewindExtensions.EnableBuffering(request);

            // IMPORTANT: Leave the body open so the next middleware can read it.
            using (
                StreamReader reader = new StreamReader(
                    request.Body,
                    Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false,
                    leaveOpen: true
                )
            )
            {
                string strRequestBody = await reader.ReadToEndAsync();
                objRequestBody = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(strRequestBody);

                // IMPORTANT: Reset the request body stream position so the next middleware can read it
                request.Body.Position = 0;
            }

            return objRequestBody;
        }
        private bool IsRoleScopeBased(string objectId, string nameId)
        {
            return !String.Equals(objectId, nameId);
        }

        private void SetExceptionMessageToHttpContext(string message)
        {
            _httpContext.HttpContext.Items[AuthorizationPolicyEvaluator.contextKey] = message;
        }

        private void BuildExceptionMessage(StringBuilder stringBuilder, string message)
        {
            stringBuilder.AppendFormat("{0} ", message);
        }

        private bool AcceptRequestUpdateUser(string objectId)
        {
            if (
                HttpMethods.IsPut(_httpContext.HttpContext.Request.Method) &&
                String.Equals(_httpContext.HttpContext.Request.RouteValues["Action"] as string, "UpdateUser")
            )
            {
                var expression = String.Equals(_httpContext.HttpContext.Request.RouteValues["userId"] as string, objectId);
                return BuildBoolExpression(expression, _stringBuilderRegular, "User ID does not match the one on requested URL!");
            }
            return true;
        }

        private async Task<bool> AcceptRequestModifyProject(User user, string objectId, IEnumerable<string> currentProjectsOfUserInDatabase)
        {
            var action = _httpContext.HttpContext.Request.RouteValues["Action"] as string;
            if (
                action.Contains("Project") &&
                !action.Contains("Get") &&
                !HttpMethods.IsGet(_httpContext.HttpContext.Request.Method)
            )
            {
                if (action.Contains("Create"))
                {
                    var projectProfile = await GetRequestBodyAsync<ProjectProfile>(_httpContext.HttpContext.Request);
                    var isReqBodyManagerIdMatchedObjectId = String.Equals(projectProfile.ProjectManager.UserID, objectId);
                    return BuildBoolExpression(
                        isReqBodyManagerIdMatchedObjectId, _stringBuilderAdmin,
                        "Manager ID in Request Body does not match object ID!"
                    );
                }
                var requestedProjectNumber = _httpContext.HttpContext.Request.RouteValues["projectNumber"] as string;
                return BuildBoolExpression(
                    currentProjectsOfUserInDatabase.Contains(requestedProjectNumber), _stringBuilderAdmin,
                    "Requested project number on URL is not owned by this user!"
                );
            }
            return true;
        }

        private bool BuildBoolExpression(bool expression, StringBuilder stringBuilder, string errorMessage)
        {
            if (!expression) BuildExceptionMessage(stringBuilder, errorMessage);
            return expression;
        }

        private bool IsRoleAdminOrManager(string[] appPermissionsOrRoles)
        {
            return (appPermissionsOrRoles.Count() == 1) && appPermissionsOrRoles.Contains("adminUser");
        }

        private bool IsRoleRegular(string[] appPermissionsOrRoles)
        {
            return (appPermissionsOrRoles.Count() == 1) && appPermissionsOrRoles.Contains("regularUser");
        }

        private bool IsScopeAdmin(string[] delegatedPermissions)
        {
            var expression = (delegatedPermissions.Count() > 1) && delegatedPermissions.Contains("Admin");
            return BuildBoolExpression(expression, _stringBuilderAdmin, "Scope is wrong!");
        }

        private bool IsScopeRegular(string[] delegatedPermissions)
        {
            // BuildExceptionMessageRegular("Regular User Scope is wrong!");
            var expression = (delegatedPermissions.Count() == 1) && delegatedPermissions.Contains("Regular");
            return BuildBoolExpression(expression, _stringBuilderRegular, "Scope is wrong!");
        }

        private bool IsUserAdminInDatabase(User user)
        {
            return user.IsAdmin;
            // return BuildBoolExpression(user.IsAdmin, _stringBuilderAdmin, "User is not an Admin in Database!");
        }

        private bool IsUserRegularInDatabase(User user)
        {
            return !user.IsManager && !user.IsAdmin;
            // return BuildBoolExpression(!user.IsManager && !user.IsAdmin, _stringBuilderRegular, "User is not a Regular in Database!");
        }

        private bool IsUserManagerOnlyInDatabase(User user)
        {
            return user.IsManager && !user.IsAdmin;
            // return BuildBoolExpression(user.IsManager && !user.IsAdmin, _stringBuilderRegular, "User is not a Manager (not Admin) in Database!");
        }

        private bool IsAuthorizedRegular(string[] delegatedPermissions, User user, string objectId)
        {
            return (
                IsScopeRegular(delegatedPermissions) &&
                IsUserRegularInDatabase(user) &&
                AcceptRequestUpdateUser(objectId)
            );
        }

        private async Task<bool> IsAuthorizedManager(string[] delegatedPermissions, User user, string objectId, IEnumerable<string> projectNumbers)
        {
            return (
                IsScopeAdmin(delegatedPermissions) &&
                await AcceptRequestModifyProject(user, objectId, projectNumbers)
            );
        }
        private void HandleAuthorized(
            bool isAuthorizedRole, StringBuilder stringBuilder,
            AuthorizationHandlerContext context, AnyValidPermissionRequirement requirement
        )
        {
            if (!isAuthorizedRole)
            {
                SetExceptionMessageToHttpContext(stringBuilder.ToString().Trim());
                context.Fail();
                return;
            }
            context.Succeed(requirement);
            return;
        }

        private async Task HandleAuthorized(
            string[] appPermissionsOrRoles, string[] delegatedPermissions, string objectId,
            AuthorizationHandlerContext context, AnyValidPermissionRequirement requirement
        )
        {
            var user = await _usersRepository.GetAUser(objectId);

            if (user == null)
            {
                SetExceptionMessageToHttpContext("Object Id is not in Database!");
                context.Fail();
                return;
            }

            if (IsRoleAdminOrManager(appPermissionsOrRoles))
            {
                if (IsUserAdminInDatabase(user))
                {
                    BuildExceptionMessage(_stringBuilderAdmin, "Admin Logged In:");
                    HandleAuthorized(IsScopeAdmin(delegatedPermissions), _stringBuilderAdmin, context, requirement);
                    return;
                }
                else if (IsUserManagerOnlyInDatabase(user))
                {
                    var projectNumbers = await _projectsRepository.GetAllProjectNumbersOfManager(objectId);
                    BuildExceptionMessage(_stringBuilderAdmin, "Project Manager Logged In:");
                    HandleAuthorized(await IsAuthorizedManager(delegatedPermissions, user, objectId, projectNumbers), _stringBuilderAdmin, context, requirement);
                    return;
                }
            }
            else if (IsRoleRegular(appPermissionsOrRoles))
            {
                BuildExceptionMessage(_stringBuilderRegular, "Regular Logged In:");
                HandleAuthorized(IsAuthorizedRegular(delegatedPermissions, user, objectId), _stringBuilderRegular, context, requirement);
                return;
            }
            else
            {
                SetExceptionMessageToHttpContext("Cannot happen!!!");
                context.Fail();
                return;
            }
        }
    }
}
