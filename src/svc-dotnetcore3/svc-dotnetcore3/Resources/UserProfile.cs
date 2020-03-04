using System.Collections.Generic;
using Web.API.Application.Models;
using AutoMapper;

namespace Web.API.Resources
{
    public class UserProfile
    {
        public UserSummary UserSummary { get; set; }
        public IEnumerable<ProjectSummary> CurrentProjects { get; set; }
        // public IEnumerable<ProjectDirectMappingResource> CurrentProjects { get; set; }

        public IEnumerable<OutOfOfficeResource> Availability { get; set; }

        public IEnumerable<ResourceDisciplineResource> Disciplines { get; set; }
        // public IEnumerable<RDisciplineResource> Disciplines { get; set; }

        public IEnumerable<PositionResource> Positions { get; set; }

    }
}
