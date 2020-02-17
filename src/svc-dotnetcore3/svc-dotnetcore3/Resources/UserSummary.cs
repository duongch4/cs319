using Web.API.Application.Models;

namespace Web.API.Resources
{
    public class UserSummary
    {
        /// <summary>User First Name</summary>
        /// <example>Elissa</example>
        public string FirstName { get; set; }
        
        /// <summary>User Last Name</summary>
        /// <example>Anderson</example>
        public string LastName { get; set; }

        /// <summary>UserName</summary>
        /// <example>eliander</example>
        public string Username { get; set; }

        /// <summary>Location Array with City and Province</summary>
        /// <example>["Vancouver", "BC"]</example>
        public Location Location { get; set; }

        /// <summary>Utilization</summary>
        /// <example>22</example>
        public int Utilization { get; set; }

    }
}
