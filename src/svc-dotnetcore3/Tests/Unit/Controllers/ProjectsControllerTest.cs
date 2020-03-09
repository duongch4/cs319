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
using Xunit;

using Serilog;

namespace Tests.Unit.Controllers
{
    public class ProjectsControllerTest
    {
        private readonly Mock<IProjectsRepository> _mockProjectsRepo;
        private readonly Mock<IUsersRepository> _mockUsersRepo;
        private readonly Mock<IPositionsRepository> _mockPositionsRepo;
        private readonly Mock<ILocationsRepository> _mockLocationsRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ProjectsController _controller;

        public ProjectsControllerTest()
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

        [Fact]
        public async void GetAllProjects_TryBlock_SearchWordIsNull()
        {
            string searchWord = null;
            var result = await _controller.GetAllProjects(searchWord, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
            _mockProjectsRepo.Verify(
                repo => repo.GetAllProjectResources(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                Times.Once
            );
            _mockProjectsRepo.Verify(
                repo => repo.GetAllProjectResourcesWithTitle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                Times.Never
            );
        }

        [Fact]
        public async void GetAllProjects_TryBlock_SearchWordIsEmptyString()
        {
            string searchWord = "";
            var result = await _controller.GetAllProjects(searchWord, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
            _mockProjectsRepo.Verify(
                repo => repo.GetAllProjectResources(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                Times.Once
            );
            _mockProjectsRepo.Verify(
                repo => repo.GetAllProjectResourcesWithTitle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                Times.Never
            );
        }

        [Fact]
        public async void GetAllProjects_TryBlock_SearchWordIsNotNullAndNotEmptyString()
        {
            string searchWord = "title";
            var result = await _controller.GetAllProjects(searchWord, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
            _mockProjectsRepo.Verify(
                repo => repo.GetAllProjectResources(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                Times.Never
            );
            _mockProjectsRepo.Verify(
                repo => repo.GetAllProjectResourcesWithTitle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                Times.Once
            );
        }

        [Fact]
        public async void GetAllProjects_TryBlock_SearchWordIsNullOrEmpty_ReturnNullProjects()
        {
            string searchWord = null;
            _mockProjectsRepo.Setup(
                repo => repo.GetAllProjectResources(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())
            ).ReturnsAsync(Enumerable.Empty<ProjectResource>());
            var result = (await _controller.GetAllProjects(searchWord, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())) as ObjectResult;
            _mockProjectsRepo.Verify(
                repo => repo.GetAllProjectResources(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                Times.Once
            );
            Assert.IsType<NotFoundException>(result.Value);
            Assert.Equal(result.StatusCode, StatusCodes.Status404NotFound);
        }

        [Fact]
        public async void GetAllProjects_TryBlock_SearchWordIsNotNullOrEmpty_ReturnNullProjects()
        {
            string searchWord = "title";
            _mockProjectsRepo.Setup(
                repo => repo.GetAllProjectResourcesWithTitle(searchWord, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())
            ).ReturnsAsync(Enumerable.Empty<ProjectResource>());
            var result = (await _controller.GetAllProjects(searchWord, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())) as ObjectResult;
            _mockProjectsRepo.Verify(
                repo => repo.GetAllProjectResourcesWithTitle(searchWord, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                Times.Once
            );
            Assert.IsType<NotFoundException>(result.Value);
            Assert.Equal(result.StatusCode, StatusCodes.Status404NotFound);
        }
    }
}