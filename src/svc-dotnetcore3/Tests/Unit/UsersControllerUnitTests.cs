using AutoMapper;
using Web.API.Application.Repository;
using Web.API.Application.Communication;
using Web.API.Resources;
using Web.API.Controllers;
using Web.API.Application.Models;
using Microsoft.AspNetCore.Mvc;
using StatusCodes = Microsoft.AspNetCore.Http.StatusCodes;
using Moq;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

using Serilog;

namespace Tests.Unit
{
    public class UsersControllerUnitTests
    {
        private readonly Mock<IUsersRepository> _mockUsersRepo;
        private readonly Mock<IProjectsRepository> _mockProjectsRepo;
        private readonly Mock<IPositionsRepository> _mockPositionsRepo;
        private readonly Mock<ILocationsRepository> _mockLocationsRepo;
        private readonly Mock<IDisciplinesRepository> _mockDisciplinesRepo;
        private readonly Mock<ISkillsRepository> _mockSkillsRepo;
        private readonly Mock<IOutOfOfficeRepository> _mockOutOfOfficeRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly UsersController _controller;
        
        public UsersControllerUnitTests() {
            _mockUsersRepo = new Mock<IUsersRepository>();
            _mockProjectsRepo = new Mock<IProjectsRepository>();
            _mockPositionsRepo = new Mock<IPositionsRepository>();
            _mockLocationsRepo = new Mock<ILocationsRepository>();
            _mockDisciplinesRepo = new Mock<IDisciplinesRepository>();
            _mockSkillsRepo = new Mock<ISkillsRepository>();
            _mockOutOfOfficeRepo = new Mock<IOutOfOfficeRepository>();
            _mockMapper = new Mock<IMapper>();
            _controller = new UsersController(
                _mockUsersRepo.Object, _mockProjectsRepo.Object, 
                _mockPositionsRepo.Object, _mockLocationsRepo.Object, 
                _mockDisciplinesRepo.Object, _mockSkillsRepo.Object, 
                _mockOutOfOfficeRepo.Object, _mockMapper.Object
            );

        }

        /********** Helper functions for verification **********/

        /********** Helper functions for mapper setup **********/
        // private void Setup_Return_Mapper_Map_FromIEnumerableProjectResource_ToIEnumerableProjectSummary(int expectedCount, IEnumerable<ProjectSummary> returnValue)
        // {
        //     _mockMapper.Setup(
        //         mapper => mapper.Map<IEnumerable<ProjectResource>, IEnumerable<ProjectSummary>>(It.Is<IEnumerable<ProjectResource>>(x => x.Count() == expectedCount))
        //     ).Returns(returnValue);
        // }

        /********** Helper function for User repo setup **********/
        private void Setup_UsersRepo_GetAllUserResourcesOnFilter_ThrowsException(System.Exception exception)
        {
            _mockUsersRepo.Setup(
                repo => repo.GetAllUserResourcesOnFilter(It.IsAny<RequestSearchUsers>())
            ).Throws(exception);
        }

        private void Setup_UsersRepo_GetAllUserResourcesOnFilter_Default(IEnumerable<UserResource> returnVal)
        {
            _mockUsersRepo.Setup(
                repo => repo.GetAllUserResourcesOnFilter(It.IsAny<RequestSearchUsers>())
            ).ReturnsAsync(returnVal);
        }

        private void Setup_UsersRepo_GetAllUserResources_ThrowsException(System.Exception exception)
        {
            _mockUsersRepo.Setup(
                repo => repo.GetAllUserResources(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())
            ).Throws(exception);
        }

        private void Setup_UsersRepo_GetAllUserResources_Default(IEnumerable<UserResource> returnVal)
        {
            _mockUsersRepo.Setup(
                repo => repo.GetAllUserResources(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())
            ).ReturnsAsync(returnVal);
        }

        private void Setup_UsersRepo_GetAUserResource_ThrowsException(System.Exception exception)
        {
            _mockUsersRepo.Setup(
                repo => repo.GetAUserResource(It.IsAny<string>())
            ).Throws(exception);
        }

        private void Setup_UsersRepo_GetAUserResource_Default(UserResource returnVal)
        {
            _mockUsersRepo.Setup(
                repo => repo.GetAUserResource(It.IsAny<string>())
            ).ReturnsAsync(returnVal);
        }

        private void Setup_UsersRepo_UpdateAUser_ThrowsException(System.Exception exception)
        {
            _mockUsersRepo.Setup(
                repo => repo.UpdateAUser(It.IsAny<UserSummary>(), It.IsAny<Location>())
            ).Throws(exception);
        }

        private void Setup_UsersRepo_UpdateAUser_Default(string returnVal)
        {
            _mockUsersRepo.Setup(
                repo => repo.UpdateAUser(It.IsAny<UserSummary>(), It.IsAny<Location>())
            ).ReturnsAsync(returnVal);
        }

        /********** Helper function for Projects repo setup **********/
        private void Setup_ProjectsRepo_GetAllProjectResourcesOfUser_ThrowException(System.Exception exception)
        {
            _mockProjectsRepo.Setup(
                repo => repo.GetAllProjectResourcesOfUser(It.IsAny<string>())
            ).Throws(exception);
        }

        private void Setup_ProjectsRepo_GetAllProjectResourcesOfUser_Default(IEnumerable<ProjectResource> returnVal)
        {
            _mockProjectsRepo.Setup(
                repo => repo.GetAllProjectResourcesOfUser(It.IsAny<string>())
            ).ReturnsAsync(returnVal);
        }

        /********** Helper function for Positions repo setup **********/
        private void Setup_PositionsRepo_GetPositionsOfUser_ThrowsException(System.Exception exception)
        {
            _mockPositionsRepo.Setup(
                repo => repo.GetPositionsOfUser(It.IsAny<string>())
            ).Throws(exception);
        }

        private void Setup_PositionsRepo_GetPositionsOfUser_Default(IEnumerable<PositionResource> returnVal)
        {
            _mockPositionsRepo.Setup(
                repo => repo.GetPositionsOfUser(It.IsAny<string>())
            ).ReturnsAsync(returnVal);
        }

        /********** Helper function for Locations repo setup **********/
        //GetAlocation
        private void Setup_LocationsRepo_GetALocation_ThrowsException(System.Exception exception)
        {
            _mockLocationsRepo.Setup(
                repo => repo.GetALocation(It.IsAny<string>())
            ).Throws(exception);
        }

        private void Setup_LocationsRepo_GetALocation_Default(Location returnVal)
        {
            _mockLocationsRepo.Setup(
                repo => repo.GetALocation(It.IsAny<string>())
            ).ReturnsAsync(returnVal);
        }

        /********** Helper function for Disciplines repo setup **********/
        private void Setup_DisciplinesRepo_GetUserDisciplines_ThrowException(System.Exception exception)
        {
            _mockDisciplinesRepo.Setup(
                repo => repo.GetUserDisciplines(It.IsAny<string>())
            ).Throws(exception);
        }

        private void Setup_DisciplinesRepo_GetUserDisciplines_Default(IEnumerable<ResourceDiscipline> returnVal)
        {
            _mockDisciplinesRepo.Setup(
                repo => repo.GetUserDisciplines(It.IsAny<string>())
            ).ReturnsAsync(returnVal);
        }

        private void Setup_DisciplinesRepo_InsertResourceDiscipline_ThrowException(System.Exception exception)
        {
            _mockDisciplinesRepo.Setup(
                repo => repo.InsertResourceDiscipline(It.IsAny<ResourceDiscipline>())
            ).Throws(exception);
        }

        private void Setup_DisciplinesRepo_InsertResourceDiscipline_Default(ResourceDiscipline returnVal)
        {
            _mockDisciplinesRepo.Setup(
                repo => repo.InsertResourceDiscipline(It.IsAny<ResourceDiscipline>())
            ).ReturnsAsync(returnVal);
        }
            // disciplinesRepo.DeleteResourceDiscipline
        
        private void Setup_DisciplinesRepo_DeleteResourceDiscipline_ThrowsException(System.Exception exception)
        {
            _mockDisciplinesRepo.Setup(
                repo => repo.DeleteResourceDiscipline(It.IsAny<ResourceDiscipline>())
            ).Throws(exception);
        }

        private void Setup_DisciplinesRepo_DeleteResourceDiscipline_Default(ResourceDiscipline returnVal)
        {
            _mockDisciplinesRepo.Setup(
                repo => repo.DeleteResourceDiscipline(It.IsAny<ResourceDiscipline>())
            ).ReturnsAsync(returnVal);
        }

        /********** Helper function for Skills repo setup **********/ 
            // skillsRepo.GetUserSkills
            // skillsRepo.InsertResourceSkill
            // skillsRepo.DeleteResourceSkill
        
        /********** Helper function for OutOfOffice repo setup **********/  
            // outOfOfficeRepo.GetAllOutOfOFficeForUser
            // outOfOfficeRepo.DeleteOutOfOffice
            // outOfOfficeRepo.InsertOutOfOffice

        /********** Tests for GetAllUsers **********/
        [Fact]
        public async void GetAllUsers_TryBlock_ReturnObjectResult()
        {
            var result = await _controller.GetAllUsers(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
            Assert.IsType<ObjectResult>(result);
        }

        /********** Tests for GetAUser **********/
        [Fact]
        public async void GetAUser_TryBlock_ReturnObjectResult()
        {
            var result = await _controller.GetAUser(It.IsAny<string>());
            Assert.IsType<ObjectResult>(result);
        }

        /********** Tests for UpdateUser **********/
        [Fact]
        public async void UpdateUser_TryBlock_ReturnObjectResult()
        {
            var result = await _controller.UpdateUser(It.IsAny<UserProfile>(), It.IsAny<string>());
            Assert.IsType<ObjectResult>(result);
        }
    
                /********** Tests for GetAUser **********/
        [Fact]
        public async void SearchUsers_TryBlock_ReturnObjectResult()
        {
            var result = await _controller.SearchUsers(It.IsAny<RequestSearchUsers>());
            Assert.IsType<ObjectResult>(result);
        }
    }
}