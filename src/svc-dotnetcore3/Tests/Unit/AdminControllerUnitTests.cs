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

        [Fact]
        public async Task CreateADiscipline_TryBlock_ReturnObjectResult() 
        {
            var result = await _controller.CreateADiscipline(It.IsAny<DisciplineResource>());
            Assert.IsType<ObjectResult>(result);
        }

        private void Verify_DisciplinesRepo_CreateADiscipline(System.Func<Times> times)
        {
            _mockDisciplinesRepo.Verify(
                repo => repo.CreateADiscipline(It.IsAny<DisciplineResource>()),
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
                repo => repo.CreateADiscipline(It.IsAny<DisciplineResource>())
            ).ReturnsAsync(0);
        }

        private async Task CreateADiscipline_TryBlock_ReturnId(DisciplineResource discipline) 
        {
            Setup_DisciplinesRepo_CreateADiscipline_Default(discipline);
            var result = (await _controller.CreateADiscipline(discipline)) as ObjectResult;
            Verify_DisciplinesRepo_CreateADiscipline(Times.Once);
            Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
            Assert.IsType<CreatedResponse<int>>(result.Value);
            var response = result.Value as CreatedResponse<int>;
            Assert.IsType<int>(response.payload);
        }

        [Fact]
        public async void CreateADiscipline_TryBlock_ReturnValidId()
        {
            var discipline = new DisciplineResource();
            await CreateADiscipline_TryBlock_ReturnId(discipline);
        }

        [Fact]
        public async void CreateADiscipline_CatchBlock_ReturnSqlException()
        {
            string errMessage = "Internal Server Error";
            var sqlException = new SqlExceptionBuilder().WithErrorNumber(50000).WithErrorMessage(errMessage).Build();
            Setup_DisciplinesRepo_CreateADiscipline_ThrowsException(sqlException);

            var result = (await _controller.CreateADiscipline(It.IsAny<DisciplineResource>())) as ObjectResult;
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

            var result = (await _controller.CreateADiscipline(It.IsAny<DisciplineResource>())) as ObjectResult;
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
    }
}