using AutoMapper;
using Web.API.Application.Models;
using Web.API.Resources;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

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
            SetOutOfOffice();
            SetRSkill();
            SetPositionSummary();
        }

        private void SetProjectSummary()
        {
            CreateMap<ProjectResource, ProjectSummary>(
            ).ForMember(
                destinationMember => destinationMember.Location,
                opt => opt.MapFrom(
                    sourceMember => new LocationResource
                    {
                        LocationID = sourceMember.LocationId,
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
                    sourceMember => (sourceMember.Skills == null) ? new HashSet<string>() : new HashSet<string>(sourceMember.Skills.Split(sep))
                )
            ).ForMember(
                destinationMember => destinationMember.YearsOfExp,
                opt => opt.MapFrom(
                    sourceMember => sourceMember.YearsOfExperience
                )
            ).ForMember(
                destinationMember => destinationMember.PositionID,
                opt => opt.MapFrom(
                    sourceMember => sourceMember.Id
                )
            ).ForMember(
                destinationMember => destinationMember.CommitmentMonthlyHours,
                opt => opt.MapFrom(
                    sourceMember => JsonConvert.DeserializeObject<Dictionary<string, int>>(sourceMember.CommitmentMonthlyHours)
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
            char[] sep = { ',' };

            CreateMap<UserResource, UserSummary>(
            ).ForMember(
                destinationMember => destinationMember.Location,
                opt => opt.MapFrom(
                    sourceMember => new LocationResource
                    {
                        LocationID = sourceMember.LocationId,
                        Province = sourceMember.Province,
                        City = sourceMember.City
                    }
                )
            ).ForMember(
                destinationMember => destinationMember.ResourceDiscipline,
                opt => opt.MapFrom(
                    sourceMember => new ResourceDisciplineResource
                    {
                        DisciplineID = sourceMember.DisciplineId,
                        Discipline = sourceMember.DisciplineName,
                        YearsOfExp = sourceMember.YearsOfExperience,
                        Skills = (sourceMember.Skills == null) ? new HashSet<string>() : new HashSet<string>(sourceMember.Skills.Split(sep))
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

        private void SetOutOfOffice()
        {
            CreateMap<OutOfOffice, OutOfOfficeResource>();
        }

        private void SetRSkill()
        {
            CreateMap<ResourceSkill, RSkillResource>();
        }

        private void SetPositionSummary()
        {
            CreateMap<PositionResource, PositionSummary>(
            ).ForMember(
                destinationMember => destinationMember.ProjectedMonthlyHours,
                opt => opt.MapFrom(
                    sourceMember => JsonConvert.DeserializeObject<Dictionary<string, int>>(sourceMember.ProjectedMonthlyHours)
                )
            ).ReverseMap();
        }
    }
}
