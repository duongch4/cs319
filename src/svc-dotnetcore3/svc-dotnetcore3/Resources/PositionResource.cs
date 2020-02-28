namespace Web.API.Resources
{
    public class PositionResource
    {
        /// <summary>Project Title</summary>
        /// <example>Secure the Time Loop</example>
        public string ProjectTitle{get; set;}
        /// <summary>Discipline Name</summary>
        /// <example>Intel</example>
        public string DisciplineName {get; set;}
        /// <summary>Committment hours</summary>
        /// <example>300</example>
        public int ProjectedMonthlyHours {get; set;}
        // public int ResourceId {get; set;}
    }
}