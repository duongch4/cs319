using System.Collections.Generic;

namespace Web.API.Resources
{
    public class OpeningPositionsSummary
    {
        /// <summary>Commitment Monthly Hours</summary>
        /// <example>160</example>
        public int CommitmentMonthlyHours { get; set; }

        /// <summary>Discipline Name</summary>
        /// <example>Intel</example>
        public string Discipline { get; set; }

        /// <summary>Years Of Experience</summary>
        /// <example>5-7</example>
        public string YearsOfExp { get; set; }

        /// <summary>Skills</summary>
        /// <example>["s1", "s2"]</example>
        public HashSet<string> Skills { get; set; }
    }
}