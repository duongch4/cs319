using System.Collections.Generic;

namespace Web.API.Resources
{
    public class RDisciplineResource
    {
        /// <summary>Discipline Name</summary>
        /// <example>Intel</example>
        public string Discipline { get; set; }
        /// <summary>Years of Experience Range</summary>
        /// <example>3-5</example>
        public string YearsOfExperience {get; set;}
        /// <summary>Skills of Discipline</summary>
        /// <example>["False Identity Creation", "Deception"]</example>
        public List<string> Skills {get; set;}
    }
}