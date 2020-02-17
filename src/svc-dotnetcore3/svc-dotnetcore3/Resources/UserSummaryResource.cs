using System.Collections.Generic;
using Web.API.Application.Models;
using System.Linq;
using AutoMapper;

namespace Web.API.Resources
{
    public class UserSummaryResource
    {
        private string name;
        private Discipline discipline;
        private Position position;
        private int utilization;
        private Location location;
        private int userId;
        private IMapper mapper;
        public UserSummaryResource(User user, Discipline discipline, Position position, IEnumerable<Position> positions, Location location, IMapper mapper){
            this.mapper = mapper;
            this.userId = user.Id;
            this.name = user.FirstName + " " + user.LastName;
            this.discipline = discipline;
            this.position = position;
            this.utilization = positions.Aggregate(0, (result, x) => result + x.ProjectedMonthlyHours);
            this.location = location;
        }
        public int UserId { get; set; }

        public string Name { get; set; }

        public DisciplineResource Discipline;

        public PositionResource Position;

        /// <summary>Location ID</summary>
        /// <example>22</example>
        public LocationResource Location {get; set;}
    }
}
