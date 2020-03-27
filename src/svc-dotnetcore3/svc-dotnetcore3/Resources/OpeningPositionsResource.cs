namespace Web.API.Resources
{
    public class OpeningPositionsResource
    {
        /// <summary>Position Id</summary>
        /// <example>1</example>
        public int Id { get; set; }
        
        /// <summary>Commitment Monthly Hours</summary>
        /// <example>160</example>
        public string CommitmentMonthlyHours { get; set; }

        /// <summary>Discipline Name</summary>
        /// <example>Intel</example>
        public string Discipline { get; set; }

        /// <summary>Skills</summary>
        /// <example>String that lists all skills separated by comma: "Deception,False Identity Creation"</example>
        public string Skills { get; set; }

        /// <summary>Years Of Experience</summary>
        /// <example>1-3</example>
        public string YearsOfExperience { get; set; }
    }
}