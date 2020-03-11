namespace Web.API.Resources
{
    public class DisciplineResource
    {
        /// <summary>Discipline Id</summary>
        /// <example>5</example>
        public int Id { get; set; }

        /// <summary>Discipline Name</summary>
        /// <example>Intel</example>
        public string Name { get; set; }

        /// <summary>Skills</summary>
        /// <example>String that lists all skills separated by comma: "Deception,False Identity Creation"</example>
        public string Skills { get; set; }
    }
}