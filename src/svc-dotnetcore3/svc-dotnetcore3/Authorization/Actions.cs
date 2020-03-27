using System.Linq;

namespace Web.API.Authorization
{
    /// <summary>
    /// Actions that can be done on the API
    /// </summary>
    internal static class Actions
    {
        public const string AdminThings = "AdminThings";
        public const string RegularThings = "RegularThings";

        public static string[] All => typeof(Actions)
            .GetFields()
            .Where(f => f.Name != nameof(All))
            .Select(f => f.GetValue(null) as string)
            .ToArray();
    }
}
