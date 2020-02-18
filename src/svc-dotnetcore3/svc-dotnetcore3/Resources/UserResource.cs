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

        /// <summary>UserName</summary>
        /// <example>eliander</example>
        public string Username { get; set; }

        /// <summary>Location ID</summary>
        /// <example>5</example>
        public int LocationId { get; set; }

        /// <summary>Location Province</summary>
        /// <example>Ontario</example>
        public string Province { get; set; }

        /// <summary>Location City</summary>
        /// <example>Toronto</example>
        public string City { get; set; }

        /// <summary>Position Confirmation Status</summary>
        /// <example>true</example>
        public string IsConfirmed { get; set; }

        /// <summary>Discipline</summary>
        /// <example>AAA</example>
        public string DisciplineName { get; set; }

        /// <summary>Years of Experience</summary>
        /// <example>10+</example>
        public string YearsOfExperience { get; set; }
    }
}
