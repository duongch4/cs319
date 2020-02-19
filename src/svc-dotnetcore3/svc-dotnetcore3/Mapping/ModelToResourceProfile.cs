using AutoMapper;
using Web.API.Application.Models;
using Web.API.Resources;
using System;
using System.Collections.Generic;

namespace Web.API.Mapping
{
    public class ModelToResourceProfile : Profile
    {
        public ModelToResourceProfile()
        {
            SetProjectSummary();
            SetUserSummary();
            SetOpeningPositionSummary();
            SetLocationProfile();
            SetUserProfile();
            SetSkillProfile();
            // SetProjectProfile();
            SetOutOfOffice();
            SetRDiscipline();
            SetRSkill();
            SetProject();
        }

        private void SetProjectSummary()
        {
            CreateMap<ProjectResource, ProjectSummary>(
            ).ForMember(
                destinationMember => destinationMember.Location,
                opt => opt.MapFrom(
                    sourceMember => new Location
                    {
                        Id = sourceMember.LocationId,
                        Province = sourceMember.Province,
                        City = sourceMember.City
                    }
                )
            ).ReverseMap();
        }


        private void SetOpeningPositionSummary()
        {
            char[] sep = { ',' };
            CreateMap<OpeningPositionsResource, OpeningPositionsSummary>(
            ).ForMember(
                destinationMember => destinationMember.Skills,
                opt => opt.MapFrom(
                    sourceMember => new HashSet<string>(sourceMember.Skills.Split(sep))
                )
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

        private void SetUserSummary()
        {
            CreateMap<UserResource, UserSummary>(
            ).ForMember(
                destinationMember => destinationMember.Location,
                opt => opt.MapFrom(
                    sourceMember => new Location
                    {
                        Id = sourceMember.LocationId,
                        Province = sourceMember.Province,
                        City = sourceMember.City
                    }
                )
            ).ForMember(
                destinationMember => destinationMember.Utilization,
                opt => opt.MapFrom(
                    sourceMember => RandomNumber(0,100)
                )
            ).ReverseMap();
        }
        private int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        private void SetSkillProfile()
        {
            CreateMap<Skill, SkillResource>(
            ).ForMember(
                destinationMember => destinationMember.Name,
                opt => opt.MapFrom(sourceMember => sourceMember.Name)
            ).ReverseMap();
        }
        private void SetProject() {
            CreateMap<Project, ProjectDirectMappingResource>();
        }

        private void SetOutOfOffice() {
            CreateMap<OutOfOffice, OutOfOfficeResource>();
        }

        private void SetRDiscipline() {
            CreateMap<ResourceDiscipline, RDisciplineResource>();
        }
        private void SetRSkill() {
            CreateMap<ResourceSkill, RSkillResource>();
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