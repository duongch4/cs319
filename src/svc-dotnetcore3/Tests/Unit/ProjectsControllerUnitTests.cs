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
    public class ProjectsControllerUnitTests
    {
        private readonly Mock<IProjectsRepository> _mockProjectsRepo;
        private readonly Mock<IUsersRepository> _mockUsersRepo;
        private readonly Mock<IPositionsRepository> _mockPositionsRepo;
        private readonly Mock<ILocationsRepository> _mockLocationsRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ProjectsController _controller;

        public ProjectsControllerUnitTests()
        {
            _mockProjectsRepo = new Mock<IProjectsRepository>();
            _mockUsersRepo = new Mock<IUsersRepository>();
            _mockPositionsRepo = new Mock<IPositionsRepository>();
            _mockLocationsRepo = new Mock<ILocationsRepository>();
            _mockMapper = new Mock<IMapper>();
            _controller = new ProjectsController(
                _mockProjectsRepo.Object, _mockUsersRepo.Object,
                _mockPositionsRepo.Object, _mockLocationsRepo.Object, _mockMapper.Object
            );
        }

        [Fact]
        public async void GetAllProjects_TryBlock_ReturnObjectResult()
        {
            var result = await _controller.GetAllProjects(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
            Assert.IsType<ObjectResult>(result);
        }

        private void Verify_ProjectsRepo_GetAllProjectResources_Default(System.Func<Times> times)
        {
            _mockProjectsRepo.Verify(
                repo => repo.GetAllProjectResources(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                times
            );
        }

        private void Verify_ProjectsRepo_GetAllProjectResourcesWithTitle_Default(System.Func<Times> times)
        {
            _mockProjectsRepo.Verify(
                repo => repo.GetAllProjectResourcesWithTitle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                times
            );
        }

        private void Setup_ProjectsRepo_GetAllProjectResources_ThrowsException(System.Exception exception)
        {
            _mockProjectsRepo.Setup(
                repo => repo.GetAllProjectResources(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())
            ).Throws(exception);
        }

        private void Setup_ProjectsRepo_GetAllProjectResourcesWithTitle_ThrowsException(System.Exception exception)
        {
            _mockProjectsRepo.Setup(
                repo => repo.GetAllProjectResourcesWithTitle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())
            ).Throws(exception);
        }

        private void Setup_Return_ProjectsRepo_GetAllProjectResources_Default(IEnumerable<ProjectResource> returnValue)
        {
            _mockProjectsRepo.Setup(
                repo => repo.GetAllProjectResources(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())
            ).ReturnsAsync(returnValue);
        }

        private void Setup_Return_ProjectsRepo_GetAllProjectResourcesWithTitle_Default(IEnumerable<ProjectResource> returnValue)
        {
            _mockProjectsRepo.Setup(
                repo => repo.GetAllProjectResourcesWithTitle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())
            ).ReturnsAsync(returnValue);
        }

        [Fact]
        public async void GetAllProjects_TryBlock_SearchWordIsNull()
        {
            string searchWord = null;
            var result = await _controller.GetAllProjects(searchWord, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
            Verify_ProjectsRepo_GetAllProjectResources_Default(Times.Once);
            Verify_ProjectsRepo_GetAllProjectResourcesWithTitle_Default(Times.Never);
        }

        [Fact]
        public async void GetAllProjects_TryBlock_SearchWordIsEmptyString()
        {
            string searchWord = "";
            var result = await _controller.GetAllProjects(searchWord, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
            Verify_ProjectsRepo_GetAllProjectResources_Default(Times.Once);
            Verify_ProjectsRepo_GetAllProjectResourcesWithTitle_Default(Times.Never);
        }

        [Fact]
        public async void GetAllProjects_TryBlock_SearchWordIsNotNullAndNotEmptyString()
        {
            string searchWord = "title";
            var result = await _controller.GetAllProjects(searchWord, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
            Verify_ProjectsRepo_GetAllProjectResources_Default(Times.Never);
            Verify_ProjectsRepo_GetAllProjectResourcesWithTitle_Default(Times.Once);
        }

        [Fact]
        public async void GetAllProjects_TryBlock_SearchWordIsNullOrEmpty_ReturnNullProjects()
        {
            string searchWord = null;
            Setup_Return_ProjectsRepo_GetAllProjectResources_Default(Enumerable.Empty<ProjectResource>());
            var result = (await _controller.GetAllProjects(searchWord, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())) as ObjectResult;
            Verify_ProjectsRepo_GetAllProjectResources_Default(Times.Once);
            Verify_ProjectsRepo_GetAllProjectResourcesWithTitle_Default(Times.Never);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
            Assert.IsType<NotFoundException>(result.Value);
        }

        [Fact]
        public async void GetAllProjects_TryBlock_SearchWordIsNotNullOrEmpty_ReturnNullProjects()
        {
            string searchWord = "title";
            Setup_Return_ProjectsRepo_GetAllProjectResourcesWithTitle_Default(Enumerable.Empty<ProjectResource>());
            var result = (await _controller.GetAllProjects(searchWord, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())) as ObjectResult;
            Verify_ProjectsRepo_GetAllProjectResources_Default(Times.Never);
            Verify_ProjectsRepo_GetAllProjectResourcesWithTitle_Default(Times.Once);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
            Assert.IsType<NotFoundException>(result.Value);
        }

        private void Setup_Return_Mapper_Map_FromIEnumerableProjectResource_ToIEnumerableProjectSummary(int expectedCount, IEnumerable<ProjectSummary> returnValue)
        {
            _mockMapper.Setup(
                mapper => mapper.Map<IEnumerable<ProjectResource>, IEnumerable<ProjectSummary>>(It.Is<IEnumerable<ProjectResource>>(x => x.Count() == expectedCount))
            ).Returns(returnValue);
        }

        private void Verify_Mapper_Map_FromIEnumerableProjectResource_ToIEnumerableProjectSummary(int expectedCount, System.Func<Times> times)
        {
            _mockMapper.Verify(
                mapper => mapper.Map<IEnumerable<ProjectResource>, IEnumerable<ProjectSummary>>(It.Is<IEnumerable<ProjectResource>>(x => x.Count() == expectedCount)),
                times
            );
        }

        private async Task GetAllProjects_TryBlock_ReturnProjects(
            int expectedCount, string searchWord,
            IEnumerable<ProjectResource> returnValue_IEnumerableProjectResource,
            IEnumerable<ProjectSummary> returnValue_IEnumerableProjectSummary
        )
        {
            Setup_Return_ProjectsRepo_GetAllProjectResourcesWithTitle_Default(returnValue_IEnumerableProjectResource);
            Setup_Return_Mapper_Map_FromIEnumerableProjectResource_ToIEnumerableProjectSummary(expectedCount, returnValue_IEnumerableProjectSummary);
            var result = (await _controller.GetAllProjects(searchWord, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())) as ObjectResult;
            Verify_Mapper_Map_FromIEnumerableProjectResource_ToIEnumerableProjectSummary(expectedCount, Times.Once);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.IsType<OkResponse<IEnumerable<ProjectSummary>>>(result.Value);
            var response = result.Value as OkResponse<IEnumerable<ProjectSummary>>;
            Assert.Equal(expectedCount, response.payload.Count());
        }

        [Fact]
        public async void GetAllProjects_TryBlock_ReturnOneProject()
        {
            var expectedCount = 1;
            string searchWord = "title";
            var returnValue_IEnumerableProjectResource = Enumerable.Empty<ProjectResource>().Append(new ProjectResource());
            var returnValue_IEnumerableProjectSummary = Enumerable.Empty<ProjectSummary>().Append(new ProjectSummary());
            await GetAllProjects_TryBlock_ReturnProjects(expectedCount, searchWord, returnValue_IEnumerableProjectResource, returnValue_IEnumerableProjectSummary);
        }

        [Fact]
        public async void GetAllProjects_TryBlock_ReturnSomeProjects()
        {
            var expectedCount = 2;
            string searchWord = "title";
            var returnValue_IEnumerableProjectResource = Enumerable.Empty<ProjectResource>().Append(new ProjectResource()).Append(new ProjectResource());
            var returnValue_IEnumerableProjectSummary = Enumerable.Empty<ProjectSummary>().Append(new ProjectSummary()).Append(new ProjectSummary());
            await GetAllProjects_TryBlock_ReturnProjects(expectedCount, searchWord, returnValue_IEnumerableProjectResource, returnValue_IEnumerableProjectSummary);
        }

        private async Task GetAllProjects_CatchBlock_SqlException(string searchWord, string errStatus)
        {
            var result = (await _controller.GetAllProjects(searchWord, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())) as ObjectResult;
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
            Assert.IsType<InternalServerException>(result.Value);
            var response = result.Value as InternalServerException;
            Assert.Equal(errStatus, response.status);  
        }

        private async Task GetAllProjects_CatchBlock_BadRequestException(string searchWord, string errStatus)
        {
            var result = (await _controller.GetAllProjects(searchWord, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())) as ObjectResult;
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.IsType<BadRequestException>(result.Value);
            var response = result.Value as BadRequestException;
            Assert.Equal(errStatus, response.status);  
        }

        [Fact]
        public async void GetAllProjects_CatchBlock_SqlException_GetAllProjectResources()
        {
            string searchWord = null;
            string errMessage = "Internal Server Error";
            var sqlException = new SqlExceptionBuilder().WithErrorNumber(50000).WithErrorMessage(errMessage).Build();
            Setup_ProjectsRepo_GetAllProjectResources_ThrowsException(sqlException);
            await GetAllProjects_CatchBlock_SqlException(searchWord, errMessage);
        }

        [Fact]
        public async void GetAllProjects_CatchBlock_SqlException_GetAllProjectResourcesWithTitle()
        {
            string searchWord = "title";
            string errMessage = "Internal Server Error";
            var sqlException = new SqlExceptionBuilder().WithErrorNumber(50000).WithErrorMessage(errMessage).Build();
            Setup_ProjectsRepo_GetAllProjectResourcesWithTitle_ThrowsException(sqlException);
            await GetAllProjects_CatchBlock_SqlException(searchWord, errMessage);
        }

        [Fact]
        public async void GetAllProjects_CatchBlock_BadRequestException_GetAllProjectResources()
        {
            string searchWord = null;
            string errMessage = "Bad Request";
            var badRequestException = new CustomException<BadRequestException>(new BadRequestException(errMessage));
            Setup_ProjectsRepo_GetAllProjectResources_ThrowsException(badRequestException);
            await GetAllProjects_CatchBlock_BadRequestException(searchWord, errMessage);
        }

        [Fact]
        public async void GetAllProjects_CatchBlock_BadRequestException_GetAllProjectResourcesWithTitle()
        {
            string searchWord = "title";
            string errMessage = "Bad Request";
            var badRequestException = new CustomException<BadRequestException>(new BadRequestException(errMessage));
            Setup_ProjectsRepo_GetAllProjectResourcesWithTitle_ThrowsException(badRequestException);
            await GetAllProjects_CatchBlock_BadRequestException(searchWord, errMessage);
        }
    }
}