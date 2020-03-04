using System.Collections.Generic;

namespace Web.API.Resources
{
    public class ResourceDisciplineResource
    {
        /// <summary>Discipline Id</summary>
        /// <example>1</example>
        public int DisciplineID { get; set; }

        /// <summary>Discipline</summary>
        /// <example>Intel</example>
        public string Discipline { get; set; }

        /// <summary>Years of Experience</summary>
        /// <example>3-5</example>
        public string YearsOfExp { get; set; }

        /// <summary>Skills of Discipline</summary>
        /// <example>["False Identity Creation", "Deception"]</example>
        public HashSet<string> Skills { get; set; }
    }
}
