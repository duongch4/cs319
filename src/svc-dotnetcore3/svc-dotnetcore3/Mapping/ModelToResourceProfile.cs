using AutoMapper;
using Web.API.Application.Models;
using Web.API.Resources;

namespace Web.API.Mapping
{
    public class ModelToResourceProfile : Profile
    {
        public ModelToResourceProfile()
        {
            // SetProjectProfile();
            SetLocationProfile();
            SetProjectSummary();
            SetUserProfile();
            SetOutOfOffice();
            SetRDiscipline();
            SetRSkill();
        }

        private void SetProjectProfile()
        {
            CreateMap<Project, ProjectResource>(
            ).ForMember(
                destinationMember => destinationMember.ProjectSummary,
                opt => opt.MapFrom(sourceMember => sourceMember)
            ).ReverseMap();
        }

        private void SetLocationProfile()
        {
            CreateMap<Location, LocationResource>();
        }

        private void SetUserProfile()
        {
            CreateMap<User, UserResource>();
        }

        private void SetProjectSummary() {
            CreateMap<Project, ProjectDirectMappingResource>();
        }

        private void SetOutOfOffice() {
            CreateMap<OutOfOffice, OutOfOfficeResource>();
        }

        private void SetRDiscipline() {
            CreateMap<ResourceDisciplines, RDisciplineResource>();
        }
        private void SetRSkill() {
            CreateMap<ResourceSkill, RSkillResource>();
        }
    }
}
