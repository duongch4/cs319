using System;
using Web.API.Application.Models;

namespace Web.API.Resources
{
    public class ProjectSummary
    {
        /// <summary>Project Title</summary>
        /// <example>Designing the new Pawnee Commons</example>
        public string Title { get; set; }

        /// <summary>Location ID</summary>
        /// <example>5</example>
        public Location Location {get; set;}        

        /// <summary>Project Start Date</summary>
        /// <example>05/29/2021 05:50:06</example>
        public DateTime ProjectStartDate {get; set;}

        /// <summary>Project End Date</summary>
        /// <example>01/29/2022 05:50:06</example>
        public DateTime ProjectEndDate {get; set;}
    }
}