namespace Web.API.Resources
{
    public class DisciplineSkillResource
    {
        /// <summary>Discipline ID</summary>
        /// <example>5</example>
        public int DisciplineId { get; set; }

        /// <summary>Skill ID</summary>
        /// <example>5</example>
        public int SkillId { get; set; }

        /// <summary> Skill Name</summary>
        /// <example>Sword fighting</example>
        public string Name { get; set; }
    }
}