namespace Web.API.Resources
{
    public class RSkillResource
    {
        /// <summary>Discipline Name</summary>
        /// <example>Weapons</example>
        public int ResourceDisciplineName { get; set; }
        /// <summary>Unique Id of the Skill</summary>
        /// <example>11</example>
        public int SkillId {get; set;}
        /// <summary>Skill Name</summary>
        /// <example>Sniper Rifle</example>
        public string Name {get; set;}
    }
}