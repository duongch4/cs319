using System.Linq;

namespace Web.API.Authorization
{
    internal static class DelegatedPermissions
    {
        public const string AdminThings = "Admin";
        public const string RegularThings = "Regular";

        public static string[] All => typeof(DelegatedPermissions)
            .GetFields()
            .Where(f => f.Name != nameof(All))
            .Select(f => f.GetValue(null) as string)
            .ToArray();
    }
}
