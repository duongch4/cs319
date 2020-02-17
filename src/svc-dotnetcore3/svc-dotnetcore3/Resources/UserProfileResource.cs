using System.Collections.Generic;
using Web.API.Application.Models;
using AutoMapper;

namespace Web.API.Resources
{
    public class UserProfileResource
    {
        private UserSummaryResource userSummary;
        private IEnumerable<ProjectDirectMappingResource> currentProjects;
        private IEnumerable<OutOfOfficeResource> availability;
        private IEnumerable<RDisciplineResource> disciplines;
        private IEnumerable<RSkillResource> skills;
        private IMapper mapper;
        public UserProfileResource(UserSummaryResource userSummary, IEnumerable<Project> projects, IEnumerable<OutOfOffice> outOfOffice, IEnumerable<ResourceDisciplines> disciplines, IEnumerable<ResourceSkill> skills, IMapper mapper) {
            this.mapper = mapper;
            this.userSummary = userSummary;
            this.currentProjects = mapper.Map<IEnumerable<Project>, IEnumerable<ProjectDirectMappingResource>>(projects);
            this.availability = mapper.Map<IEnumerable<OutOfOffice>, IEnumerable<OutOfOfficeResource>>(outOfOffice);
            this.disciplines = mapper.Map<IEnumerable<ResourceDisciplines>, IEnumerable<RDisciplineResource>>(disciplines);
            this.skills = mapper.Map<IEnumerable<ResourceSkill>, IEnumerable<RSkillResource>>(skills);
        }
        public UserSummaryResource UserSummary {get; set;}
        public IEnumerable<ProjectDirectMappingResource> Projects {get; set;}

        public IEnumerable<OutOfOfficeResource> Availability {get; set;}

        public IEnumerable<DisciplineResource> Disciplines {get; set;}

    }
}
