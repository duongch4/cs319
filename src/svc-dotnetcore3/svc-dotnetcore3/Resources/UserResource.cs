namespace Web.API.Resources
{
    public class UserResource
    {
        /// <summary>User ID</summary>
        /// <example>567</example>
        public int Id { get; set; }
        /// <summary>User First Name</summary>
        /// <example>Elissa</example>
        public string FirstName { get; set; }
        /// <summary>User Last Name</summary>
        /// <example>Anderson</example>
        public string LastName { get; set; }

        /// <summary>Location ID</summary>
        /// <example>22</example>
        public int LocationId {get; set;}
    }
}
