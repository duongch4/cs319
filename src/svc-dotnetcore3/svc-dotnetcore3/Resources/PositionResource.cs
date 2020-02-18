namespace Web.API.Resources
{
    public class PositionResource
    {
        /// <summary>Position ID</summary>
        /// <example>1234</example>
        public int PositionId {get; set;}
        /// <summary>Discipline ID</summary>
        /// <example></example>
        public DisciplineResource Discipline {get; set;}
        // public int ProjectId {get; set;}
        /// <summary>Committment hours</summary>
        /// <example>300</example>
        public int ProjectedHours {get; set;}
        // public int ResourceId {get; set;}
    }
}