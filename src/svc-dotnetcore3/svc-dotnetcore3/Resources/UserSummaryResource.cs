using System.Collections.Generic;
using Web.API.Application.Models;
using System.Linq;
using AutoMapper;

namespace Web.API.Resources
{
    public class UserSummaryResource
    {
        public int userID { get; set; }

        public string firstName { get; set; }
        public string lastName { get; set; }

        public DisciplineResource Discipline;

        public PositionResource Position;

        /// <summary>Location ID</summary>
        /// <example>22</example>
        public LocationResource Location {get; set;}
        public int Utilization {get; set;}
        public bool isConfirmed {get; set;}
    }
}
