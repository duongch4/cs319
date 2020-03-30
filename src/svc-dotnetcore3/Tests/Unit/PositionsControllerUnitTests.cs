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
    public class PositionsControllerUnitTests
    {
        private readonly Mock<IUsersRepository> _mockUsersRepo;
        private readonly Mock<IProjectsRepository> _mockProjectsRepo;
        private readonly Mock<IPositionsRepository> _mockPositionsRepo;
        private readonly Mock<IOutOfOfficeRepository> _mockOutOfOfficeRepo;
        private readonly Mock<IUtilizationRepository> _mockUtilizationRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly PositionsController _controller;
        
        public PositionsControllerUnitTests() {
            _mockUsersRepo = new Mock<IUsersRepository>();
            _mockProjectsRepo = new Mock<IProjectsRepository>();
            _mockPositionsRepo = new Mock<IPositionsRepository>();
            _mockUtilizationRepo = new Mock<IUtilizationRepository>();
            _mockOutOfOfficeRepo = new Mock<IOutOfOfficeRepository>();
            _mockMapper = new Mock<IMapper>();
            _controller = new PositionsController(
                _mockProjectsRepo.Object, _mockUsersRepo.Object,  
                _mockPositionsRepo.Object, _mockOutOfOfficeRepo.Object,
                _mockUtilizationRepo.Object, _mockMapper.Object
            );
        }

        /********** Helper functions for User Repo setup **********/
        private void Setup_UsersRepo_GetAUser_Default(User returnVal)
        {
            _mockUsersRepo.Setup(
                repo => repo.GetAUser(It.IsAny<string>())
            ).ReturnsAsync(returnVal);
        }

        private void Setup_UsersRepo_GetAUser_ThrowsException(System.Exception exception)
        {
            _mockUsersRepo.Setup(
                repo => repo.GetAUser(It.IsAny<string>())
            ).Throws(exception);
        }

        private void Setup_UsersRepo_UpdateUtilizationOfUser_Default(User returnVal)
        {
            _mockUsersRepo.Setup(
                repo => repo.UpdateUtilizationOfUser(It.IsAny<int>(), It.IsAny<string>())
            ).ReturnsAsync(returnVal);
        }

        private void Setup_UsersRepo_UpdateUtilizationOfUser_ThrowsException(System.Exception exception)
        {
            _mockUsersRepo.Setup(
                repo => repo.UpdateUtilizationOfUser(It.IsAny<int>(), It.IsAny<string>())
            ).Throws(exception);
        }

        /********** Helper functions for Positions Repo setup **********/
        private void Setup_PositionsRepo_GetAPosition_Default(Position returnVal)
        {
            _mockPositionsRepo.Setup(
                repo => repo.GetAPosition(It.IsAny<int>())
            ).ReturnsAsync(returnVal);
        }
        
        private void Setup_PositionsRepo_GetAPosition_ThrowsException(System.Exception exception)
        {
            _mockPositionsRepo.Setup(
                repo => repo.GetAPosition(It.IsAny<int>())
            ).Throws(exception);
        }

        private void Setup_PositionsRepo_UpdateAPosition_Default(Position returnVal)
        {
            _mockPositionsRepo.Setup(
                repo => repo.UpdateAPosition(It.IsAny<Position>())
            ).ReturnsAsync(returnVal);
        }

        private void Setup_PositionsRepo_UpdateAPosition_ThrowsException(System.Exception exception)
        {
            _mockPositionsRepo.Setup(
                repo => repo.UpdateAPosition(It.IsAny<Position>())
            ).Throws(exception);
        }

        private void Setup_PositionsRepo_GetAllPositionsOfUser_Default(IEnumerable<Position> returnVal)
        {
            _mockPositionsRepo.Setup(
                repo => repo.GetAllPositionsOfUser(It.IsAny<string>())
            ).ReturnsAsync(returnVal);
        }

        private void Setup_PositionsRepo_GetAllPositionsOfUser_ThrowsException(System.Exception exception)
        {
            _mockPositionsRepo.Setup(
                repo => repo.GetAllPositionsOfUser(It.IsAny<string>())
            ).Throws(exception);
        }
        
        /********** Helper functions for OutOfOffice Repo setup **********/
        private void Setup_OutOfOfficeRepo_GetAllOutOfOfficeForUser_Default(IEnumerable<OutOfOffice> returnVal)
        {
            _mockOutOfOfficeRepo.Setup(
                repo => repo.GetAllOutOfOfficeForUser(It.IsAny<string>())
            ).ReturnsAsync(returnVal);
        }

        private void Setup_OutOfOfficeRepo_GetAllOutOfOfficeForUser_ThrowsException(System.Exception exception)
        {
            _mockOutOfOfficeRepo.Setup(
                repo => repo.GetAllOutOfOfficeForUser(It.IsAny<string>())
            ).Throws(exception);
        }

        /********** Helper functions for Utilization Repo setup **********/
        private void Setup_UtilizationRepo_CalculateUtilizationOfUser_Default(int returnVal)
        {
            _mockUtilizationRepo.Setup(
                repo => repo.CalculateUtilizationOfUser(It.IsAny<IEnumerable<Position>>(), It.IsAny<IEnumerable<OutOfOffice>>())
            ).ReturnsAsync(returnVal);
        }

        private void Setup_UtilizationRepo_CalculateUtilizationOfUser_ThrowsException(System.Exception exception)
        {
            _mockUtilizationRepo.Setup(
                repo => repo.CalculateUtilizationOfUser(It.IsAny<IEnumerable<Position>>(), It.IsAny<IEnumerable<OutOfOffice>>())
            ).Throws(exception);
        }

        /********** Tests for AssignAResource **********/
        [Fact]
        private async void AssignAResource_TryBlock_ReturnsObjectResult()
        {
            var result = await _controller.AssignAResource(It.IsAny<int>(), It.IsAny<string>());
            Assert.IsType<ObjectResult>(result);
        }

        [Fact]
        private async void AssignAResource_NullOpeningCheck_ReturnBadRequestException()
        {
            var errMessage = "Bad Request";

            var result = (await _controller.AssignAResource(0, "id")) as ObjectResult;
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.IsType<BadRequestException>(result.Value);
            var response = result.Value as BadRequestException;
            Assert.Equal(errMessage, response.status);
        }

        [Fact]
        private async void AssignAResource_NullUserCheck_ReturnBadRequestException()
        {
            var errMessage = "Bad Request";

            var result = (await _controller.AssignAResource(1, null)) as ObjectResult;
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.IsType<BadRequestException>(result.Value);
            var response = result.Value as BadRequestException;
            Assert.Equal(errMessage, response.status);
        }

        [Fact]
        private async void AssignAResource_EmptyUserCheck_ReturnBadRequestException()
        {
            var errMessage = "Bad Request";

            var result = (await _controller.AssignAResource(1, "")) as ObjectResult;
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.IsType<BadRequestException>(result.Value);
            var response = result.Value as BadRequestException;
            Assert.Equal(errMessage, response.status);
        }

        [Fact]
        private async void AssignAResource_TryBlock_PositionNull_ReturnNotFoundException()
        {
            var errMessage = "Not Found";
            Position position = null;
            Setup_PositionsRepo_GetAPosition_Default(position);
            Setup_UsersRepo_GetAUser_Default(new User());

            var result = (await _controller.AssignAResource(1, "1")) as ObjectResult;
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
            Assert.IsType<NotFoundException>(result.Value);
            var response = result.Value as NotFoundException;
            Assert.Equal(errMessage, response.status);
        }

        [Fact]
        private async void AssignAResource_TryBlock_UserNull_ReturnNotFoundException()
        {
            var errMessage = "Not Found";
            User user = null;
            Setup_PositionsRepo_GetAPosition_Default(new Position());
            Setup_UsersRepo_GetAUser_Default(user);

            var result = (await _controller.AssignAResource(1, "1")) as ObjectResult;
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
            Assert.IsType<NotFoundException>(result.Value);
            var response = result.Value as NotFoundException;
            Assert.Equal(errMessage, response.status);
        }

        // !!! TODO
        private async void AssignAResource_TryBlock_ValidAction_ReturnRequestProjectAssign(){}

        [Fact]
        private async void AssignAResource_CatchBlock_GetPositionErr_ReturnSqlException()
        {
            string errMessage = "Internal Server Error";
            var sqlException = new SqlExceptionBuilder().WithErrorNumber(50000).WithErrorMessage(errMessage).Build();
            Setup_PositionsRepo_GetAPosition_ThrowsException(sqlException);
            Setup_UsersRepo_GetAUser_Default(new User());
            Setup_PositionsRepo_UpdateAPosition_Default(new Position());

            var result = (await _controller.AssignAResource(1, "1")) as ObjectResult;
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
            var response = result.Value as InternalServerException;
            Assert.Equal(errMessage, response.status);
        }

        [Fact]
        private async void AssignAResource_CatchBlock_UserErr_ReturnSqlException()
        {
            string errMessage = "Internal Server Error";
            var sqlException = new SqlExceptionBuilder().WithErrorNumber(50000).WithErrorMessage(errMessage).Build();
            Setup_PositionsRepo_GetAPosition_Default(new Position());
            Setup_UsersRepo_GetAUser_ThrowsException(sqlException);
            Setup_PositionsRepo_UpdateAPosition_Default(new Position());

            var result = (await _controller.AssignAResource(1, "1")) as ObjectResult;
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
            var response = result.Value as InternalServerException;
            Assert.Equal(errMessage, response.status);
        }

        [Fact]
        private async void AssignAResource_CatchBlock_UpdatePositionErr_ReturnSqlException()
        {
            string errMessage = "Internal Server Error";
            var sqlException = new SqlExceptionBuilder().WithErrorNumber(50000).WithErrorMessage(errMessage).Build();
            Setup_PositionsRepo_GetAPosition_Default(new Position());
            Setup_UsersRepo_GetAUser_Default(new User());
            Setup_PositionsRepo_UpdateAPosition_ThrowsException(sqlException);

            var result = (await _controller.AssignAResource(1, "1")) as ObjectResult;
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
            var response = result.Value as InternalServerException;
            Assert.Equal(errMessage, response.status);
        } 

        [Fact]
        private async void AssignAResource_CatchBlock_GetPositionErr_ReturnBadRequestException()
        {
            string errMessage = "Bad Request";
            var badRequestException = new CustomException<BadRequestException>(new BadRequestException(errMessage));
            Setup_PositionsRepo_GetAPosition_ThrowsException(badRequestException);
            Setup_UsersRepo_GetAUser_Default(new User());
            Setup_PositionsRepo_UpdateAPosition_Default(new Position());

            var result = (await _controller.AssignAResource(1, "1")) as ObjectResult;
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.IsType<BadRequestException>(result.Value);
            var response = result.Value as BadRequestException;
            Assert.Equal(errMessage, response.status);  
        }

        [Fact]
        private async void AssignAResource_CatchBlock_UserErr_ReturnBadRequestException()
        {            
            string errMessage = "Bad Request";
            var badRequestException = new CustomException<BadRequestException>(new BadRequestException(errMessage));
            Setup_PositionsRepo_GetAPosition_Default(new Position());
            Setup_UsersRepo_GetAUser_ThrowsException(badRequestException);
            Setup_PositionsRepo_UpdateAPosition_Default(new Position());

            var result = (await _controller.AssignAResource(1, "1")) as ObjectResult;
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.IsType<BadRequestException>(result.Value);
            var response = result.Value as BadRequestException;
            Assert.Equal(errMessage, response.status);  }

        [Fact]
        private async void AssignAResource_CatchBlock_UpdatePositionErr_ReturnBadRequestException()
        {   
            string errMessage = "Bad Request";
            var badRequestException = new CustomException<BadRequestException>(new BadRequestException(errMessage));
            Setup_PositionsRepo_GetAPosition_Default(new Position());
            Setup_UsersRepo_GetAUser_Default(new User());
            Setup_PositionsRepo_UpdateAPosition_ThrowsException(badRequestException);

            var result = (await _controller.AssignAResource(1, "1")) as ObjectResult;
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.IsType<BadRequestException>(result.Value);
            var response = result.Value as BadRequestException;
            Assert.Equal(errMessage, response.status);  }
        
        /********** Tests for ConfirmResource **********/
        [Fact]
        private async void ConfirmResource_TryBlock_ReturnsObjectResult()
        {
            var result = await _controller.ConfirmResource(It.IsAny<int>());
            Assert.IsType<ObjectResult>(result);
        }

    }
}