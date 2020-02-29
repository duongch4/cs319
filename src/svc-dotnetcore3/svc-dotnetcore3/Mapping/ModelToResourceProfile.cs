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
            SetProjectManager();
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
                    sourceMember => new LocationResource
                    {
                        Province = sourceMember.Province,
                        City = sourceMember.City
                    }
                )
            ).ForMember(
                destinationMember => destinationMember.ProjectNumber,
                opt => opt.MapFrom(
                    sourceMember => sourceMember.Number
                )
            ).ReverseMap();
        }

        private void SetProjectManager()
        {
            CreateMap<ProjectResource, ProjectManager>(
            ).ForMember(
                destinationMember => destinationMember.UserID,
                opt => opt.MapFrom(
                    sourceMember => sourceMember.ManagerId
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
            ).ForMember(
                destinationMember => destinationMember.YearsOfExp,
                opt => opt.MapFrom(
                    sourceMember => sourceMember.YearsOfExperience
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
                    sourceMember => new LocationResource
                    {
                        Province = sourceMember.Province,
                        City = sourceMember.City
                    }
                )
            ).ForMember(
                destinationMember => destinationMember.Utilization,
                opt => opt.MapFrom(
                    sourceMember => RandomNumber(0, 150)
                )
            ).ForMember(
                destinationMember => destinationMember.ResourceDiscipline,
                opt => opt.MapFrom(
                    sourceMember => new ResourceDisciplineResource
                    {
                        Discipline = sourceMember.DisciplineName,
                        YearsOfExp = sourceMember.YearsOfExperience,
                    }
                )
            ).ForMember(
                destinationMember => destinationMember.UserID,
                opt => opt.MapFrom(
                    sourceMember => sourceMember.Id
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
        private void SetProject()
        {
            CreateMap<Project, ProjectDirectMappingResource>(
            ).ForMember(
                destinationMember => destinationMember.ProjectNumber,
                opt => opt.MapFrom(
                    sourceMember => sourceMember.Number
                )
            );
        }

        private void SetOutOfOffice()
        {
            CreateMap<OutOfOffice, OutOfOfficeResource>();
        }

        private void SetRDiscipline()
        {
            CreateMap<ResourceDiscipline, RDisciplineResource>();
        }
        private void SetRSkill()
        {
            CreateMap<ResourceSkill, RSkillResource>();
        }
    }
}
