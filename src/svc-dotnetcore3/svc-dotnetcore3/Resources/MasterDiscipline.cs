using System.Collections.Generic;

namespace Web.API.Resources
{
    public class MasterDiscipline
    {
        /// <summary>Discipline Id</summary>
        /// <example>1</example>
        public int DisciplineID { get; set; }
        
        /// <summary>Skills of Discipline</summary>
        /// <example>["False Identity Creation", "Deception"]</example>
        public IEnumerable<string> Skills { get; set; }
    }
}