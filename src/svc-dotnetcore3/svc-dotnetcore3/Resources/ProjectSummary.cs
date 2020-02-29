using System;
using Web.API.Application.Models;

namespace Web.API.Resources
{
    public class ProjectSummary
    {
        /// <summary>Project Title</summary>
        /// <example>Designing the new Pawnee Commons</example>
        public string Title { get; set; }

        /// <summary>Location Object with City and Province</summary>
        public LocationResource Location { get; set; }

        /// <summary>Project Start Date</summary>
        /// <example>05/29/2021T05:50:06</example>
        public DateTime ProjectStartDate { get; set; }

        /// <summary>Project End Date</summary>
        /// <example>01/29/2022T05:50:06</example>
        public DateTime ProjectEndDate { get; set; }

        /// <summary>Project Number</summary>
        /// <example>0000-0000-00</example>
        public string ProjectNumber { get; set; }
    }
}