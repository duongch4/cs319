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
    public class AdminControllerUnitTests
    {
        private readonly Mock<ILocationsRepository> _mockLocationsRepo;
        private readonly Mock<IDisciplinesRepository> _mockDisciplinesRepo;
        private readonly Mock<ISkillsRepository> _mockSkillsRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly AdminController _controller;

        public AdminControllerUnitTests()
        {
            _mockLocationsRepo = new Mock<ILocationsRepository>();
            _mockDisciplinesRepo = new Mock<IDisciplinesRepository>();
            _mockSkillsRepo = new Mock<ISkillsRepository>();
            _mockMapper = new Mock<IMapper>();
            _controller = new AdminController(
                _mockLocationsRepo.Object, _mockDisciplinesRepo.Object,
                _mockSkillsRepo.Object, _mockMapper.Object
            );
        }
        
        // Helper functions for repo set up and verification
        private void Verify_DisciplinesRepo_CreateADiscipline(System.Func<Times> times)
        {
            _mockDisciplinesRepo.Verify(
                repo => repo.CreateADiscipline(It.IsAny<DisciplineResource>()),
                times
            );
        }

        private void Verify_SkillsRepo_CreateASkill(System.Func<Times> times)
        {
            _mockSkillsRepo.Verify(
                repo => repo.CreateASkill(It.IsAny<DisciplineSkillResource>()),
                times
            );
        }
        
        private void Verify_LocationsRepo_CreateALocation(System.Func<Times> times)
        {
            _mockLocationsRepo.Verify(
                repo => repo.CreateALocation(It.IsAny<LocationResource>()),
                times
            );
        }

        private void Verify_LocationsRepo_CreateAProvince(System.Func<Times> times)
        {
            _mockLocationsRepo.Verify(
                repo => repo.CreateAProvince(It.IsAny<string>()),
                times
            );
        }

        private void Setup_DisciplinesRepo_CreateADiscipline_ThrowsException(System.Exception exception)
        {
            _mockDisciplinesRepo.Setup(
                repo => repo.CreateADiscipline(It.IsAny<DisciplineResource>())
            ).Throws(exception);
        }

        private void Setup_DisciplinesRepo_CreateADiscipline_Default(DisciplineResource returnVal)
        {
            _mockDisciplinesRepo.Setup(
                repo => repo.CreateADiscipline(returnVal)
            ).ReturnsAsync(0);
        }

        private void Setup_SkillsRepo_CreateASkill_Default(DisciplineSkillResource returnVal)
        {
            _mockSkillsRepo.Setup(
                repo => repo.CreateASkill(returnVal)
            ).ReturnsAsync(0);
        }

        private void Setup_SkillsRepo_CreateASkill_ThrowsException(System.Exception exception)
        {
            _mockSkillsRepo.Setup(
                repo => repo.CreateASkill(It.IsAny<DisciplineSkillResource>())
            ).Throws(exception);
        }

        private void Setup_LocationsRepo_CreateALocation_Default(LocationResource returnVal)
        {
            _mockLocationsRepo.Setup(
                repo => repo.CreateALocation(returnVal)
            ).ReturnsAsync(0);
        }

        private void Setup_LocationsRepo_CreateALocation_ThrowsException(System.Exception exception)
        {
            _mockLocationsRepo.Setup(
                repo => repo.CreateALocation(It.IsAny<LocationResource>())
            ).Throws(exception);
        }

        private void Setup_LocationsRepo_CreateAProvince_Default(string returnVal)
        {
            _mockLocationsRepo.Setup(
                repo => repo.CreateAProvince(returnVal)
            ).ReturnsAsync(returnVal);
        }

        private void Setup_LocationsRepo_CreateAProvince_ThrowsException(System.Exception exception)
        {
            _mockLocationsRepo.Setup(
                repo => repo.CreateAProvince(It.IsAny<string>())
            ).Throws(exception);
        }

        // Tests for Discipline Creation
        [Fact]
        public async Task CreateADiscipline_TryBlock_ReturnObjectResult() 
        {
            var result = await _controller.CreateADiscipline(It.IsAny<DisciplineResource>());
            Assert.IsType<ObjectResult>(result);
        }

        [Fact]
        public async void CreateADiscipline_TryBlock_ReturnValidId()
        {
            var discipline = new DisciplineResource();
            Setup_DisciplinesRepo_CreateADiscipline_Default(discipline);
            var result = (await _controller.CreateADiscipline(discipline)) as ObjectResult;
            Verify_DisciplinesRepo_CreateADiscipline(Times.Once);
            Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
            Assert.IsType<CreatedResponse<int>>(result.Value);
            var response = result.Value as CreatedResponse<int>;
            Assert.IsType<int>(response.payload);
        }

        [Fact]
        public async void CreateADiscipline_CatchBlock_ReturnSqlException()
        {
            string errMessage = "Internal Server Error";
            var sqlException = new SqlExceptionBuilder().WithErrorNumber(50000).WithErrorMessage(errMessage).Build();
            Setup_DisciplinesRepo_CreateADiscipline_ThrowsException(sqlException);

            var discipline = new DisciplineResource();
            var result = (await _controller.CreateADiscipline(discipline)) as ObjectResult;
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
            var response = result.Value as InternalServerException;
            Assert.Equal(errMessage, response.status);
        }

        [Fact]
        public async void CreateADiscipline_CatchBlock_ReturnBadRequestException()
        {
            string errMessage = "Bad Request";
            var badRequestException = new CustomException<BadRequestException>(new BadRequestException(errMessage));
            Setup_DisciplinesRepo_CreateADiscipline_ThrowsException(badRequestException);

            var discipline = new DisciplineResource();
            var result = (await _controller.CreateADiscipline(discipline)) as ObjectResult;
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.IsType<BadRequestException>(result.Value);
            var response = result.Value as BadRequestException;
            Assert.Equal(errMessage, response.status);
        }

        [Fact]
        public async void CreateADiscipline_NullCheck_ReturnBadRequestException()
        {
            string errMessage = "Bad Request";
            var badRequestException = new CustomException<BadRequestException>(new BadRequestException(errMessage));
            Setup_DisciplinesRepo_CreateADiscipline_ThrowsException(badRequestException);

            var result = (await _controller.CreateADiscipline(null)) as ObjectResult;
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.IsType<BadRequestException>(result.Value);
            var response = result.Value as BadRequestException;
            Assert.Equal(errMessage, response.status);
        }

        // Tests for Skill creation
        [Fact]
        private async void CreateASkill_TryBlock_ReturnValidId()
        {
            var skill = new DisciplineSkillResource{
                DisciplineId= 1,
                SkillId= 0,
                Name = ""};
            Setup_SkillsRepo_CreateASkill_Default(skill);
            var result = (await _controller.CreateASkill(skill, 1)) as ObjectResult;
            Verify_SkillsRepo_CreateASkill(Times.Once);
            Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
            Assert.IsType<CreatedResponse<int>>(result.Value);
            var response = result.Value as CreatedResponse<int>;
            Assert.IsType<int>(response.payload);
        }

        [Fact]
        private async void CreateASkill_DisciplineInvalid_BadRequestException()
        {
            var errMessage = "Bad Request";
            var badRequestException = new CustomException<BadRequestException>(new BadRequestException(errMessage));
            Setup_SkillsRepo_CreateASkill_ThrowsException(badRequestException);
            var skill = new DisciplineSkillResource{
                DisciplineId= 0,
                SkillId= 0,
                Name = ""};

            var result = (await _controller.CreateASkill(skill, 0)) as ObjectResult; 
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.IsType<BadRequestException>(result.Value);
            var response = result.Value as BadRequestException;
            Assert.Equal(errMessage, response.status);           
        }

        [Fact]
        private async void CreateASkill_DisciplineMisMatch_BadRequestException()
        {
            var errMessage = "Bad Request";
            var badRequestException = new CustomException<BadRequestException>(new BadRequestException(errMessage));
            Setup_SkillsRepo_CreateASkill_ThrowsException(badRequestException);
            var skill = new DisciplineSkillResource{
                DisciplineId= 2,
                SkillId= 0,
                Name = ""};

            var result = (await _controller.CreateASkill(skill, 9)) as ObjectResult; 
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.IsType<BadRequestException>(result.Value);
            var response = result.Value as BadRequestException;
            Assert.Equal(errMessage, response.status);           
        }

        [Fact]
        private async void CreateASkill_SkillNull_BadRequestException()
        {
            var errMessage = "Bad Request";
            var badRequestException = new CustomException<BadRequestException>(new BadRequestException(errMessage));
            Setup_SkillsRepo_CreateASkill_ThrowsException(badRequestException);

            var result = (await _controller.CreateASkill(null, 1)) as ObjectResult; 
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.IsType<BadRequestException>(result.Value);
            var response = result.Value as BadRequestException;
            Assert.Equal(errMessage, response.status);  
        }

        [Fact]
        public async void CreateASkill_CatchBlock_ReturnSqlException()
        {
            string errMessage = "Internal Server Error";
            var sqlException = new SqlExceptionBuilder().WithErrorNumber(50000).WithErrorMessage(errMessage).Build();
            Setup_SkillsRepo_CreateASkill_ThrowsException(sqlException);
            var skill = new DisciplineSkillResource{
                DisciplineId= 2,
                SkillId= 0,
                Name = ""};
            
            var result = (await _controller.CreateASkill(skill, 2)) as ObjectResult;
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
            var response = result.Value as InternalServerException;
            Assert.Equal(errMessage, response.status);
        }

        [Fact]
        public async void CreateASkill_CatchBlock_ReturnBadException()
        {
            var errMessage = "Bad Request";
            var badRequestException = new CustomException<BadRequestException>(new BadRequestException(errMessage));
            Setup_SkillsRepo_CreateASkill_ThrowsException(badRequestException);
            var skill = new DisciplineSkillResource{
                DisciplineId= 2,
                SkillId= 0,
                Name = ""};
            
            var result = (await _controller.CreateASkill(skill, 2)) as ObjectResult;
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.IsType<BadRequestException>(result.Value);
            var response = result.Value as BadRequestException;
            Assert.Equal(errMessage, response.status);
        }

        [Fact]
        public async void CreateALocation_NullCheck_ReturnBadRequestException()
        {
            string errMessage = "Bad Request";
            var badRequestException = new CustomException<BadRequestException>(new BadRequestException(errMessage));
            Setup_LocationsRepo_CreateALocation_ThrowsException(badRequestException);

            var result = (await _controller.CreateALocation(null)) as ObjectResult;
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.IsType<BadRequestException>(result.Value);
            var response = result.Value as BadRequestException;
            Assert.Equal(errMessage, response.status);
        }

        [Fact]
        public async void CreateALocation_TryBlock_ReturnValidId()
        {
            var location = new LocationResource();
            Setup_LocationsRepo_CreateALocation_Default(location);
            
            var result = (await _controller.CreateALocation(location)) as ObjectResult;
            Verify_LocationsRepo_CreateALocation(Times.Once);
            Verify_LocationsRepo_CreateAProvince(Times.Never);
            Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
            Assert.IsType<CreatedResponse<int>>(result.Value);
            var response = result.Value as CreatedResponse<int>;
            Assert.IsType<int>(response.payload); 
        }
    
        [Fact]
        public async void CreateALocation_CatchBlock_ReturnsSqlException()
        {
            string errMessage = "Internal Server Error";
            var sqlException = new SqlExceptionBuilder().WithErrorNumber(50000).WithErrorMessage(errMessage).Build();
            Setup_LocationsRepo_CreateALocation_ThrowsException(sqlException);

            var location = new LocationResource();
            var result = (await _controller.CreateALocation(location)) as ObjectResult;
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
            var response = result.Value as InternalServerException;
            Assert.Equal(errMessage, response.status);
        }

        [Fact]
        public async void CreateALocation_CatchBlock_ReturnsBadRequestException()
        {
            string errMessage = "Bad Request";
            var badRequestException = new CustomException<BadRequestException>(new BadRequestException(errMessage));
            Setup_LocationsRepo_CreateALocation_ThrowsException(badRequestException);

            var location = new LocationResource();
            var result = (await _controller.CreateALocation(location)) as ObjectResult;
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.IsType<BadRequestException>(result.Value);
            var response = result.Value as BadRequestException;
            Assert.Equal(errMessage, response.status);
        }
    }
}