using AutoMapper;
using Web.API.Application.Models;
using Web.API.Resources;

namespace Web.API.Mapping
{
    public class ModelToResourceProfile : Profile
    {
        public ModelToResourceProfile()
        {
            ProjectProfile();
        }

        private void ProjectProfile()
        {
            CreateMap<Project, ProjectResource>(
            ).ForMember(
                destinationMember => destinationMember.TitleResource,
                opt => opt.MapFrom(sourceMember => sourceMember.Title)
            ).ReverseMap();
        }
    }
}
