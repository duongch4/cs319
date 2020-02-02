using AutoMapper;
using Web.API.Application.Models;
using Web.API.Resources;

namespace Web.API.Mapping
{
    public class ModelToResourceProfile : Profile
    {
        public ModelToResourceProfile()
        {
            SetProjectProfile();
            SetLocationProfile();
            SetUserProfile();
        }

        private void SetProjectProfile()
        {
            CreateMap<Project, ProjectResource>(
            ).ForMember(
                destinationMember => destinationMember.TitleResource,
                opt => opt.MapFrom(sourceMember => sourceMember.Title)
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
    }
}
