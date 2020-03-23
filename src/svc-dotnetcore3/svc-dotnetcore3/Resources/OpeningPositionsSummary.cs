using System.Collections.Generic;
using System.Text.Json;

namespace Web.API.Resources
{
    public class OpeningPositionsSummary
    {
        /// <summary>Position Id</summary>
        /// <example>1</example>
        public int PositionID { get; set; }
        
        /// <summary>Commitment Monthly Hours</summary>
        /// <example>160</example>
        public JsonElement CommitmentMonthlyHours { get; set; }

        /// <summary>Discipline Name</summary>
        /// <example>Language</example>
        public string Discipline { get; set; }

        /// <summary>Years Of Experience</summary>
        /// <example>5-7</example>
        public string YearsOfExp { get; set; }

        /// <summary>Skills</summary>
        /// <example>["Russian", "Mandarin"]</example>
        public HashSet<string> Skills { get; set; }
    }
}