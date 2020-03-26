using System.Linq;

namespace Web.API.Authorization
{
    internal static class ApplicationPermissions
    {
        public const string DoAllAdminThings = "All.Admin.Things";
        public const string DoAllRegularThings = "All.Regular.Things";

        public const string RoleAdmin = "adminUser";
        public const string RoleRegular = "regularUser";

        public static string[] All => typeof(ApplicationPermissions)
            .GetFields()
            .Where(f => f.Name != nameof(All))
            .Select(f => f.GetValue(null) as string)
            .ToArray();
    }
}
