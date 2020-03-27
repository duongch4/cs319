using System.Text.Json;

namespace Web.API.Resources
{
    public class PositionResource
    {
        /// <summary>Position Id</summary>
        /// <example>1</example>
        public int PositionID { get; set; }

        /// <summary>Position Name</summary>
        /// <example>Time Cop</example>
        public string PositionName { get; set; }
        
        /// <summary>Project Title</summary>
        /// <example>Secure the Time Loop</example>
        public string ProjectTitle { get; set; }
        
        /// <summary>Discipline Name</summary>
        /// <example>Intel</example>
        public string DisciplineName { get; set; }
        
        /// <summary>Committment hours</summary>
        /// <example>300</example>
        public string ProjectedMonthlyHours { get; set; }
    }
}