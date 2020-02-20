namespace Web.API.Resources
{
    public class OpeningPositionsResource
    {
        /// <summary>Commitment Monthly Hours</summary>
        /// <example>160</example>
        public int CommitmentMonthlyHours { get; set; }

        /// <summary>Discipline Name</summary>
        /// <example>Intel</example>
        public string Discipline { get; set; }

        /// <summary>Skills</summary>
        /// <example>["Deception","False Identity Creation"]</example>
        public string Skills { get; set; }

        /// <summary>Years Of Experience</summary>
        /// <example>1-3</example>
        public string YearsOfExperience { get; set; }
    }
}