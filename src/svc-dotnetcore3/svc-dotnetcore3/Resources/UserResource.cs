namespace Web.API.Resources
{
    public class UserResource
    {
        /// <summary>User Id</summary>
        /// <example>5</example>
        public int Id { get; set; }

        /// <summary>User First Name</summary>
        /// <example>Sameen</example>
        public string FirstName { get; set; }
        
        /// <summary>User Last Name</summary>
        /// <example>Shaw</example>
        public string LastName { get; set; }

        /// <summary>UserName</summary>
        /// <example>shaw</example>
        public string Username { get; set; }

        /// <summary>Location Id</summary>
        /// <example>8</example>
        public int LocationId { get; set; }

        /// <summary>Location Province</summary>
        /// <example>British Columbia</example>
        public string Province { get; set; }

        /// <summary>Location City</summary>
        /// <example>Vancouver</example>
        public string City { get; set; }

        /// <summary>Position Confirmation Status</summary>
        /// <example>true</example>
        public bool IsConfirmed { get; set; }

        /// <summary>Utilization</summary>
        /// <example>22</example>
        public int Utilization { get; set; }

        /// <summary>Discipline Id</summary>
        /// <example>1</example>
        public int DisciplineId { get; set; }

        /// <summary>Discipline</summary>
        /// <example>Weapons</example>
        public string DisciplineName { get; set; }

        /// <summary>Years of Experience</summary>
        /// <example>10+</example>
        public string YearsOfExperience { get; set; }

        /// <summary>Skills</summary>
        /// <example>String that lists all skills separated by comma: "Deception,False Identity Creation"</example>
        public string Skills { get; set; }
    }
}
