using System.Collections.Generic;

namespace Web.API.Resources
{
    public class RDisciplineResource
    {
        /// <summary>Discipline Name</summary>
        /// <example>Parks and Recreation</example>
        public string Name { get; set; }
        public string YearsOfExperience {get; set;}
        public List<string> Skills {get; set;}
    }
}