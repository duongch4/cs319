namespace Web.API.Resources
{
    public class ProjectManagerResource
    {
        /// <summary>PM Id</summary>
        /// <example>1</example>
        public int UserID { get; set; }

        /// <summary>PM First Name</summary>
        /// <example>Designing the new Pawnee Commons</example>
        public string FirstName { get; set; }

        /// <summary>PM Last Name</summary>
        /// <example>5</example>
        public string LastName { get; set; }
    }
}
