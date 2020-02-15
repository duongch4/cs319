namespace Web.API.Resources
{
    public class SkillResource
    {
        /// <summary> Skill ID</summary>
        /// <example>935</example>
        public int Id {get; set;}
        /// <summary>Discipline ID</summary>
        /// <example>123</example>
        public int DisciplineId {get; set;}        
        /// <summary> Skill Name</summary>
        /// <example>Sword fighting</example>
        public string Name {get; set;}
    }
}