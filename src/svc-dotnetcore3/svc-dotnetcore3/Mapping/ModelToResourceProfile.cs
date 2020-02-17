using AutoMapper;
using Web.API.Application.Models;
using Web.API.Resources;

namespace Web.API.Mapping
{
    public class ModelToResourceProfile : Profile
    {
        public ModelToResourceProfile()
        {
            // SetProjectSummary();
            SetLocationProfile();
            SetUserProfile();
            SetSkillProfile();
        }

        // private void SetProjectSummary()
        // {
        //     CreateMap<Project, ProjectSummary>(
        //     ).ForMember(
        //         destinationMember => destinationMember,
        //         opt => opt.MapFrom(sourceMember => sourceMember)
        //     ).ReverseMap();
        // }

        private void SetLocationProfile()
        {
            CreateMap<Location, LocationResource>();
        }

        private void SetUserProfile()
        {
            CreateMap<User, UserResource>();
        }

        // private void SetUserSummary()
        // {
        //     CreateMap<User, UserSummary>();
        // }

        private void SetSkillProfile()
        {
            CreateMap<Skill, SkillResource>(
            ).ForMember(
                destinationMember => destinationMember.Name,
                opt => opt.MapFrom(sourceMember => sourceMember.Name)
            ).ReverseMap();
        }
    }
}

//UserProfile { UserSummary, Disciplines[ ], Projects[ ], Availability[ ] },
//ProjectProfile {  ProjectSummary, UserSummary[ ], Positions[ ] },

// var config = new MapperConfiguration(cfg => {
//                 cfg.CreateMap<AuthorDTO, AuthorModel>()
//                    .ForMember(destination => destination.Address,
//               map => map.MapFrom(
//                   source => new Address
//                   {
//                       City = source .City,
//                       State = source .State,
//                       Country = source.Country
//                   }));