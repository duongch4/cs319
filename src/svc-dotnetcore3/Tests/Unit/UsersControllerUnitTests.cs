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
        private readonly Mock<IUtilizationRepository> _mockUtilizationRepo;
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
            _mockUtilizationRepo = new Mock<IUtilizationRepository>();
            _mockMapper = new Mock<IMapper>();
            _controller = new UsersController(
                _mockUsersRepo.Object, _mockProjectsRepo.Object, 
                _mockPositionsRepo.Object, _mockLocationsRepo.Object, 
                _mockDisciplinesRepo.Object, _mockSkillsRepo.Object, 
                _mockOutOfOfficeRepo.Object, _mockUtilizationRepo.Object, _mockMapper.Object
            );

        }

        /********** Helper functions for mapper setup **********/
        private void Setup_Return_Mapper_Map_FromIEnumerableUserResource_ToIEnumerableUserSummary(int expectedCount, IEnumerable<UserSummary> returnVal)
        {
            _mockMapper.Setup(
                mapper => mapper.Map<IEnumerable<UserResource>, IEnumerable<UserSummary>>(It.Is<IEnumerable<UserResource>>(x => x.Count() == expectedCount))
            ).Returns(returnVal);
        }

        private void Setup_Return_Mapper_Map_FromUserResource_ToUserSummary(UserSummary returnVal)
        {
            _mockMapper.Setup(
                mapper => mapper.Map<UserResource, UserSummary>(It.IsAny<UserResource>())
            ).Returns(returnVal);
        }

        private void Setup_Return_Mapper_Map_FromIEnumerablePositionResource_ToIEnumerablePositionSummary(int expectedCount, IEnumerable<PositionSummary> returnVal)
        {
            _mockMapper.Setup(
                mapper => mapper.Map<IEnumerable<PositionResource>, IEnumerable<PositionSummary>>(It.Is<IEnumerable<PositionResource>>(x => x.Count() == expectedCount))
            ).Returns(returnVal);
        }

        private void Setup_Return_Mapper_Map_FromIEnumerableOutOfOffice_ToIEnumerableOutOfOfficeResource(int expectedCount, IEnumerable<OutOfOfficeResource> returnVal)
        {
            _mockMapper.Setup(
                mapper => mapper.Map<IEnumerable<OutOfOffice>, IEnumerable<OutOfOfficeResource>>(It.Is<IEnumerable<OutOfOffice>>(x => x.Count() == expectedCount))
            ).Returns(returnVal);
        }
        
        private void Setup_Return_Mapper_Map_FromIEnumerableProjectResource_ToIEnumerableProjectSummary(int expectedCount, IEnumerable<ProjectSummary> returnValue)
        {
            _mockMapper.Setup(
                mapper => mapper.Map<IEnumerable<ProjectResource>, IEnumerable<ProjectSummary>>(It.Is<IEnumerable<ProjectResource>>(x => x.Count() == expectedCount))
            ).Returns(returnValue);
        }
        
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
        private void Setup_SkillsRepo_GetUserSkills_ThrowsException(System.Exception exception)
        {
            _mockSkillsRepo.Setup(
                repo => repo.GetUserSkills(It.IsAny<string>())
            ).Throws(exception);
        }

        private void Setup_SkillsRepo_GetUserSkills_Default(IEnumerable<ResourceSkill> returnVal)
        {
            _mockSkillsRepo.Setup(
                repo => repo.GetUserSkills(It.IsAny<string>())
            ).ReturnsAsync(returnVal);
        }

        private void Setup_SkillsRepo_InsertResourceSkill_ThrowsException(System.Exception exception)
        {
            _mockSkillsRepo.Setup(
                repo => repo.InsertResourceSkill(It.IsAny<ResourceSkill>())
            ).Throws(exception);
        }

        private void Setup_SkillsRepo_InsertResourceSkill_Default(ResourceSkill returnVal)
        {
            _mockSkillsRepo.Setup(
                repo => repo.InsertResourceSkill(It.IsAny<ResourceSkill>())
            ).ReturnsAsync(returnVal);
        }

        private void Setup_SkillsRepo_DeleteResourceSkill_ThrowsException(System.Exception exception)
        {
            _mockSkillsRepo.Setup(
                repo => repo.DeleteResourceSkill(It.IsAny<ResourceSkill>())
            ).Throws(exception);
        }

        private void Setup_SkillsRepo_DeleteResourceSkill_Default(ResourceSkill returnVal)
        {
            _mockSkillsRepo.Setup(
                repo => repo.DeleteResourceSkill(It.IsAny<ResourceSkill>())
            ).ReturnsAsync(returnVal);
        }
        
        /********** Helper function for OutOfOffice repo setup **********/  
        private void Setup_OutOfOfficeRepo_GetAllOutOfOfficeForUser_ThrowsException(System.Exception exception)
        {
            _mockOutOfOfficeRepo.Setup(
                repo => repo.GetAllOutOfOfficeForUser(It.IsAny<string>())
            ).Throws(exception);
        }

        private void Setup_OutOfOfficeRepo_GetAllOutOfOfficeForUser_Default(IEnumerable<OutOfOffice> returnVal)
        {
            _mockOutOfOfficeRepo.Setup(
                repo => repo.GetAllOutOfOfficeForUser(It.IsAny<string>())
            ).ReturnsAsync(returnVal);
        }

        private void Setup_OutOfOfficeRepo_DeleteOutOfOffice_ThrowsException(System.Exception exception)
        {
            _mockOutOfOfficeRepo.Setup(
                repo => repo.DeleteOutOfOffice(It.IsAny<OutOfOffice>())
            ).Throws(exception);
        }

        private void Setup_OutOfOfficeRepo_DeleteOutOfOffice_Default(OutOfOffice returnVal)
        {
            _mockOutOfOfficeRepo.Setup(
                repo => repo.DeleteOutOfOffice(It.IsAny<OutOfOffice>())
            ).ReturnsAsync(returnVal);
        }
            // outOfOfficeRepo.InsertOutOfOffice
        private void Setup_OutOfOffice_InsertOutOfOffice_ThrowsException(System.Exception exception)
        {
            _mockOutOfOfficeRepo.Setup(
                repo => repo.InsertOutOfOffice(It.IsAny<OutOfOffice>())
            ).Throws(exception);
        }

        private void Setup_OutOfOffice_InsertOutOfOffice_Default(OutOfOffice returnVal)
        {
            _mockOutOfOfficeRepo.Setup(
                repo => repo.InsertOutOfOffice(It.IsAny<OutOfOffice>())
            ).ReturnsAsync(returnVal);
        }

        private void Setup_AllRepo_UpdateUser(ResourceDiscipline discipline, IEnumerable<ResourceDiscipline> dbDisciplines,
            OutOfOffice availability, IEnumerable<OutOfOffice> dbAvailability, ResourceSkill skill, IEnumerable<ResourceSkill> dbSkills) {

            // Disciplines Repo
            Setup_DisciplinesRepo_GetUserDisciplines_Default(dbDisciplines);
            Setup_DisciplinesRepo_DeleteResourceDiscipline_Default(discipline);
            Setup_DisciplinesRepo_InsertResourceDiscipline_Default(discipline);

            // OutOfOffice repo
            Setup_OutOfOfficeRepo_GetAllOutOfOfficeForUser_Default(dbAvailability);
            Setup_OutOfOffice_InsertOutOfOffice_Default(availability);
            Setup_OutOfOfficeRepo_DeleteOutOfOffice_Default(availability);

            // Skills repo
            Setup_SkillsRepo_GetUserSkills_Default(dbSkills);
            Setup_SkillsRepo_DeleteResourceSkill_Default(skill);
            Setup_SkillsRepo_InsertResourceSkill_Default(skill);
        }

        /********** Tests for GetAllUsers **********/
        [Fact]
        public async void GetAllUsers_TryBlock_ReturnObjectResult()
        {
            var result = await _controller.GetAllUsers(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
            Assert.IsType<ObjectResult>(result);
        }

        [Fact]
        public async void GetAllUsers_TryBlock_NullCheck_ReturnsNotFoundException()
        {
            IEnumerable<UserResource> expected = null;
            Setup_UsersRepo_GetAllUserResources_Default(expected);
            var result = (await _controller.GetAllUsers(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())) as ObjectResult;
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
            Assert.IsType<NotFoundException>(result.Value);
        }

        [Fact]
        public async void GetAllUsers_TryBlock_ReturnOneUser()
        {
            var expectedCount = 1;
            var returnVal_IEnumerableUserResource = Enumerable.Empty<UserResource>().Append(new UserResource());
            var returnVal_IEnumerableUserSummary = Enumerable.Empty<UserSummary>().Append(new UserSummary());
            Setup_UsersRepo_GetAllUserResources_Default(returnVal_IEnumerableUserResource);
            Setup_Return_Mapper_Map_FromIEnumerableUserResource_ToIEnumerableUserSummary(expectedCount, returnVal_IEnumerableUserSummary);

            var result = (await _controller.GetAllUsers(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())) as ObjectResult;

            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.IsType<OkResponse<IEnumerable<UserSummary>>>(result.Value);
            var response = result.Value as OkResponse<IEnumerable<UserSummary>>;
            Assert.Equal(expectedCount, response.payload.Count());
        }

        [Fact]
        public async void GetAllUsers_TryBlock_ReturnSomeUsers()
        {
            var expectedCount = 3;
            var returnVal_IEnumerableUserResource = Enumerable.Empty<UserResource>().Append(new UserResource()).Append(new UserResource()).Append(new UserResource());
            var returnVal_IEnumerableUserSummary = Enumerable.Empty<UserSummary>().Append(new UserSummary()).Append(new UserSummary()).Append(new UserSummary());
            Setup_UsersRepo_GetAllUserResources_Default(returnVal_IEnumerableUserResource);
            Setup_Return_Mapper_Map_FromIEnumerableUserResource_ToIEnumerableUserSummary(expectedCount, returnVal_IEnumerableUserSummary);

            var result = (await _controller.GetAllUsers(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())) as ObjectResult;

            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.IsType<OkResponse<IEnumerable<UserSummary>>>(result.Value);
            var response = result.Value as OkResponse<IEnumerable<UserSummary>>;
            Assert.Equal(expectedCount, response.payload.Count());
        }

        [Fact]
        public async void GetAllUsers_CatchBlock_ReturnSqlException()
        {
            string errMessage = "Internal Server Error";
            var sqlException = new SqlExceptionBuilder().WithErrorNumber(50000).WithErrorMessage(errMessage).Build();
            Setup_UsersRepo_GetAllUserResources_ThrowsException(sqlException);

            var result = (await _controller.GetAllUsers(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())) as ObjectResult;
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
            var response = result.Value as InternalServerException;
            Assert.Equal(errMessage, response.status);
        }

        [Fact]
        public async void GetAllUsers_CatchBlock_ReturnBadRequestException()
        {
            string errMessage = "Bad Request";
            var badRequestException = new CustomException<BadRequestException>(new BadRequestException(errMessage));
            Setup_UsersRepo_GetAllUserResources_ThrowsException(badRequestException);

            var result = (await _controller.GetAllUsers(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())) as ObjectResult;
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.IsType<BadRequestException>(result.Value);
            var response = result.Value as BadRequestException;
            Assert.Equal(errMessage, response.status);  
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
    
        [Fact]
        public async void UpdateUser_TryBlock_NullCheck_ReturnBadRequestException(){
            var errMessage = "Bad Request";

            var result = (await _controller.UpdateUser(null, "test id")) as ObjectResult;
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.IsType<BadRequestException>(result.Value);
            var response = result.Value as BadRequestException;
            Assert.Equal(errMessage, response.status);
        }

        [Fact]
        public async void UpdateUser_TryBlock_InvalidUser_ReturnInternalServerException(){
            Setup_LocationsRepo_GetALocation_Default(new Location());
            Setup_UsersRepo_UpdateAUser_Default("-1");
            var errMessage = "Internal Server Error";
            var location = new LocationResource {
                LocationID = 1,
                City = "city",
                Province = "province"
            };
            var summary = new UserSummary {
                UserID = "1",
                Location = location,
                FirstName = "first",
                LastName = "last",
                Utilization = 0,
                ResourceDiscipline = null,
                IsConfirmed = false
            };
            var user = new UserProfile {
                UserSummary = summary,
                CurrentProjects = Enumerable.Empty<ProjectSummary>(),
                Availability = Enumerable.Empty<OutOfOfficeResource>(),
                Disciplines = Enumerable.Empty<ResourceDisciplineResource>(),
                Positions = Enumerable.Empty<PositionSummary>()
            };

            var result = (await _controller.UpdateUser(user, "1")) as ObjectResult;
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
            var response = result.Value as InternalServerException;
            Assert.Equal(errMessage, response.status);
        }

        [Fact]
        public async void UpdateUser_TryBlock_ReturnValidUserID(){
            Setup_AllRepo_UpdateUser(new ResourceDiscipline(), Enumerable.Empty<ResourceDiscipline>(),
                    new OutOfOffice(), Enumerable.Empty<OutOfOffice>(), new ResourceSkill(), Enumerable.Empty<ResourceSkill>());
            Setup_LocationsRepo_GetALocation_Default(new Location());
            Setup_UsersRepo_UpdateAUser_Default("1");

            var location = new LocationResource {
                LocationID = 1,
                City = "city",
                Province = "province"
            };
            var summary = new UserSummary {
                UserID = "1",
                Location = location,
                FirstName = "first",
                LastName = "last",
                Utilization = 0,
                ResourceDiscipline = null,
                IsConfirmed = false
            };
            var user = new UserProfile {
                UserSummary = summary,
                CurrentProjects = Enumerable.Empty<ProjectSummary>(),
                Availability = Enumerable.Empty<OutOfOfficeResource>(),
                Disciplines = Enumerable.Empty<ResourceDisciplineResource>(),
                Positions = Enumerable.Empty<PositionSummary>()
            };
            
            var result = (await _controller.UpdateUser(user, "1")) as ObjectResult;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.IsType<OkResponse<string>>(result.Value);
            var response = result.Value as OkResponse<string>;
            Assert.IsType<string>(response.payload);
        }

        [Fact]
        public async void UpdateUser_CatchBlock_ReturnInternalServerException(){
            var errMessage = "Internal Server Error";
            var sqlException = new SqlExceptionBuilder().WithErrorNumber(50000).WithErrorMessage(errMessage).Build();
            Setup_AllRepo_UpdateUser(It.IsAny<ResourceDiscipline>(), Enumerable.Empty<ResourceDiscipline>(),
                    new OutOfOffice(), Enumerable.Empty<OutOfOffice>(), new ResourceSkill(), Enumerable.Empty<ResourceSkill>());
            Setup_LocationsRepo_GetALocation_Default(new Location());
            Setup_UsersRepo_UpdateAUser_ThrowsException(sqlException);

            var location = new LocationResource {
                LocationID = 1,
                City = "city",
                Province = "province"
            };
            var summary = new UserSummary {
                UserID = "NoneEmpty",
                Location = location,
                FirstName = "first",
                LastName = "last",
                Utilization = 0,
                ResourceDiscipline = null,
                IsConfirmed = false
            };
            var user = new UserProfile {
                UserSummary = summary,
                CurrentProjects = Enumerable.Empty<ProjectSummary>(),
                Availability = Enumerable.Empty<OutOfOfficeResource>(),
                Disciplines = Enumerable.Empty<ResourceDisciplineResource>(),
                Positions = Enumerable.Empty<PositionSummary>()
            };
            
            var result = (await _controller.UpdateUser(user, "NoneEmpty")) as ObjectResult;
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
            var response = result.Value as InternalServerException;
            Assert.Equal(errMessage, response.status);
        }

        [Fact]
        public async void UpdateUser_CatchBlock_ReturnBadRequestException(){
            string errMessage = "Bad Request";
            var badRequestException = new CustomException<BadRequestException>(new BadRequestException(errMessage));
            Setup_UsersRepo_UpdateAUser_ThrowsException(badRequestException);

            var location = new LocationResource {
                LocationID = 1,
                City = "city",
                Province = "province"
            };
            var summary = new UserSummary {
                UserID = null,
                Location = location,
                FirstName = "first",
                LastName = "last",
                Utilization = 0,
                ResourceDiscipline = null,
                IsConfirmed = false
            };
            var user = new UserProfile {
                UserSummary = summary,
                CurrentProjects = Enumerable.Empty<ProjectSummary>(),
                Availability = Enumerable.Empty<OutOfOfficeResource>(),
                Disciplines = Enumerable.Empty<ResourceDisciplineResource>(),
                Positions = Enumerable.Empty<PositionSummary>()
            };
            
            var result = (await _controller.UpdateUser(user, "")) as ObjectResult;
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.IsType<BadRequestException>(result.Value);
            var response = result.Value as BadRequestException;
            Assert.Equal(errMessage, response.status);  
        }
        /********** Tests for SearchUsers **********/
        [Fact]
        public async void SearchUsers_TryBlock_ReturnObjectResult()
        {
            var result = await _controller.SearchUsers(It.IsAny<RequestSearchUsers>());
            Assert.IsType<ObjectResult>(result);
        }
    }
}