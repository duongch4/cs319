using Web.API.Application.Models;

namespace Web.API.Resources
{
    public class UserSummary
    {
        /// <summary>UserName</summary>
        /// <example>eliander</example>
        public string UserID { get; set; }

        /// <summary>User First Name</summary>
        /// <example>Elissa</example>
        public string FirstName { get; set; }
        
        /// <summary>User Last Name</summary>
        /// <example>Anderson</example>
        public string LastName { get; set; }

        /// <summary>Location Object with City and Province</summary>
        /// <example>["Vancouver", "BC"]</example>
        public LocationResource Location { get; set; }

        /// <summary>Utilization</summary>
        /// <example>22</example>
        public int Utilization { get; set; }

        /// <summary>Resource Discipline</summary>
        /// <example>2</example>
        public ResourceDisciplineResource ResourceDiscipline { get; set; }

        /// <summary>Confirmation Status</summary>
        /// <example>true</example>
        public bool IsConfirmed { get; set; }
    }
}
