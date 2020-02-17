namespace Web.API.Resources
{
    public class OpeningPositionsResource
    {
        /// <summary>Commitment Monthly Hours</summary>
        /// <example>160</example>
        public int CommitmentMonthlyHours { get; set; }

        /// <summary>Position Name</summary>
        /// <example>Intel</example>
        public string Position { get; set; }

        /// <summary>Discipline Name</summary>
        /// <example>Intel</example>
        public string Discipline { get; set; }

        /// <summary>Skills</summary>
        /// <example>["s1","s2"]</example>
        public string Skills { get; set; }

        /// <summary>Years Of Experience</summary>
        /// <example>5-7</example>
        public string YearsOfExperience { get; set; }
    }
}