using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StatusCodes = Microsoft.AspNetCore.Http.StatusCodes;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.API.Application.Models;
using Web.API.Application.Repository;
using Web.API.Application.Communication;
using Web.API.Authorization;
using Web.API.Resources;
using System.Text.Json;

using System;
using System.Data.SqlClient;
using Serilog;

namespace Web.API.Controllers
{
    [Authorize(Actions.AdminThings)]
    [Route("api")]
    [Produces("application/json")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class PositionsController : ControllerBase
    {
        private readonly IProjectsRepository projectsRepository;
        private readonly IUsersRepository usersRepository;
        private readonly IPositionsRepository positionsRepository;
        private readonly IOutOfOfficeRepository outOfOfficeRepository;
        private readonly IUtilizationRepository utilizationRepository;
        private readonly IMapper mapper;

        public PositionsController(
            IProjectsRepository projectsRepository, IUsersRepository usersRepository,
            IPositionsRepository positionsRepository,
            IOutOfOfficeRepository outOfOfficeRepository,
            IUtilizationRepository utilizationRepository,
            IMapper mapper
        )
        {
            this.projectsRepository = projectsRepository;
            this.usersRepository = usersRepository;
            this.positionsRepository = positionsRepository;
            this.outOfOfficeRepository = outOfOfficeRepository;
            this.utilizationRepository = utilizationRepository;
            this.mapper = mapper;
        }

        /// <summary>Assign a Resource to a Position</summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/positions/25/assign/15
        ///
        /// </remarks>
        /// <param name="openingId">Id of position to assign resource to</param>
        /// <param name="userId">Id of resource to assign to position</param>
        /// <returns>The Id of the opening that a resource has been assigned to,
        ///          The Id of the resource that has been assigned, and
        ///          The (confirmed) utilization of the resource that has been assigned</returns>
        /// <response code="200">Returns openingId, resourceId, resource's utilization</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">If either the opening or user cannot be found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPut]
        [Route("positions/{openingId}/assign/{userId}")]
        [ProducesResponseType(typeof(OkResponse<RequestProjectAssign>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AssignAResource([FromRoute] int openingId, string userId)
        {
            try
            {
                if (openingId == 0)
                {
                    var error = new BadRequestException($"The given opening {openingId} is invalid");
                    return StatusCode(StatusCodes.Status400BadRequest, new CustomException<BadRequestException>(error).GetException());
                }

                if (String.IsNullOrEmpty(userId))
                {
                    var error = new BadRequestException("The given user is invalid");
                    return StatusCode(StatusCodes.Status400BadRequest, new CustomException<BadRequestException>(error).GetException());
                }

                Position position = await positionsRepository.GetAPosition(openingId);
                User user = await usersRepository.GetAUser(userId);

                if (position == null)
                {
                    var error = new NotFoundException($"Invalid positionId {openingId}.");
                    return StatusCode(StatusCodes.Status404NotFound, new CustomException<NotFoundException>(error).GetException());
                }
                if (user == null)
                {
                    var error = new NotFoundException($"Resource with id {userId} not found.");
                    return StatusCode(StatusCodes.Status404NotFound, new CustomException<NotFoundException>(error).GetException());

                }

                // Log.Information("{@response}", position);

                position.ResourceId = userId;
                position.IsConfirmed = false;
                var updatedPosition = await positionsRepository.UpdateAPosition(position);



                RequestProjectAssign response = new RequestProjectAssign
                {
                    OpeningId = openingId,
                    UserID = userId,
                    ConfirmedUtilization = user.Utilization
                };

                // Log.Information("{@response}", response);
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception err)
            {
                var errMessage = $"Source: {err.Source}\n  Message: {err.Message}\n  StackTrace: {err.StackTrace}\n";
                if (err is SqlException)
                {
                    var error = new InternalServerException(errMessage);
                    return StatusCode(StatusCodes.Status500InternalServerError, new CustomException<InternalServerException>(error).GetException());
                }
                else
                {
                    var error = new BadRequestException(errMessage);
                    return StatusCode(StatusCodes.Status400BadRequest, new CustomException<BadRequestException>(error).GetException());
                }
            }
        }

        /// <summary>Confirms a Resource on a Position</summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/positions/25/confirm
        ///
        /// </remarks>
        /// <param name="openingId">Id of position that is to be confirmed</param>
        /// <returns>The Id of the opening that a resource has been assigned to,
        ///          The Id of the resource that has been confirmed, and
        ///          The (confirmed) utilization of the resource that has been confirmed</returns>
        /// <response code="200">Returns openingId, resourceId, resource's utilization</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">If the opening cannot be found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPut]
        [Route("positions/{openingId}/confirm")]
        [ProducesResponseType(typeof(OkResponse<RequestProjectAssign>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ConfirmResource([FromRoute] int openingId)
        {

            if (openingId == 0)
            {
                var error = new BadRequestException($"The given opening {openingId} is invalid");
                return StatusCode(StatusCodes.Status400BadRequest, new CustomException<BadRequestException>(error).GetException());
            }
            try
            {
                Position position = await positionsRepository.GetAPosition(openingId);

                if (position == null)
                {
                    var error = new NotFoundException($"Invalid positionId {openingId}.");
                    return StatusCode(StatusCodes.Status404NotFound, new CustomException<NotFoundException>(error).GetException());
                }

                if (position.ResourceId == null || position.ResourceId == "")
                {
                    var error = new BadRequestException($"Position {position.Id} does not have a resource assigned.");
                    return StatusCode(StatusCodes.Status400BadRequest, new CustomException<BadRequestException>(error).GetException());
                }

                position.IsConfirmed = true;
                var updatedPosition = await positionsRepository.UpdateAPosition(position);

                IEnumerable<Position> positionsOfUser = await positionsRepository.GetAllPositionsOfUser(position.ResourceId);
                IEnumerable<OutOfOffice> outOfOfficesOfUser = await outOfOfficeRepository.GetAllOutOfOfficeForUser(position.ResourceId);

                var utilization = await utilizationRepository.CalculateUtilizationOfUser(positionsOfUser, outOfOfficesOfUser);
                await usersRepository.UpdateUtilizationOfUser(utilization, position.ResourceId);

                RequestProjectAssign response = new RequestProjectAssign
                {
                    OpeningId = openingId,
                    UserID = position.ResourceId,
                    ConfirmedUtilization = utilization
                };
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception err)
            {
                var errMessage = $"Source: {err.Source}\n  Message: {err.Message}\n  StackTrace: {err.StackTrace}\n";
                if (err is SqlException)
                {
                    var error = new InternalServerException(errMessage);
                    return StatusCode(StatusCodes.Status500InternalServerError, new CustomException<InternalServerException>(error).GetException());
                }
                else
                {
                    var error = new BadRequestException(errMessage);
                    return StatusCode(StatusCodes.Status400BadRequest, new CustomException<BadRequestException>(error).GetException());
                }
            }
        }

        /// <summary>Unassign a Resource to a Position</summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/positions/25/unassign/
        ///
        /// </remarks>
        /// <param name="openingId">Id of position that we're unassign resource from</param>
        /// <returns>The OpeningId of the opening that we're unassigning a resource from,
        ///          The UserId of the Resource that we unassigned
        ///          The updated ConfirmedUtilization of the Resource that we unassigned
        ///          The OpeningPositionsSummary of the position that we unassigned from</returns>
        /// <response code="200">Returns RequestUnassign of opening</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">If opening cannot be found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPut]
        [Route("positions/{openingId}/unassign/")]
        [ProducesResponseType(typeof(OkResponse<RequestProjectAssign>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UnassignResource([FromRoute] int openingId)
        {
            if (openingId == 0)
            {
                var error = new BadRequestException($"The given opening {openingId} is invalid");
                return StatusCode(StatusCodes.Status400BadRequest, new CustomException<BadRequestException>(error).GetException());
            }
            try
            {
                Position position = await positionsRepository.GetAPosition(openingId);

                if (position.ResourceId == null)
                {
                    var error = new BadRequestException($"Position does not have a resource to unassign");
                    return StatusCode(StatusCodes.Status400BadRequest, new CustomException<BadRequestException>(error).GetException());
                }

                User user = await usersRepository.GetAUser(position.ResourceId);

                if (position == null)
                {
                    var error = new NotFoundException($"Invalid positionId {openingId}.");
                    return StatusCode(StatusCodes.Status404NotFound, new CustomException<NotFoundException>(error).GetException());
                }
                if (user == null)
                {
                    var error = new NotFoundException($"Resource with id {position.ResourceId} not found.");
                    return StatusCode(StatusCodes.Status404NotFound, new CustomException<NotFoundException>(error).GetException());

                }
                position.ResourceId = null;
                position.IsConfirmed = false;

                position = await positionsRepository.UpdateAPosition(position);

                IEnumerable<Position> positions = await positionsRepository.GetAllPositionsOfUser(user.Id);
                IEnumerable<OutOfOffice> outOfOffices = await outOfOfficeRepository.GetAllOutOfOfficeForUser(user.Id);

                int newUtilizationOfUser = await utilizationRepository.CalculateUtilizationOfUser(positions, outOfOffices);
                user = await usersRepository.UpdateUtilizationOfUser(newUtilizationOfUser, user.Id);

                OpeningPositionsResource openingRes = await positionsRepository.GetAnOpeningPositionsResource(openingId);
                OpeningPositionsSummary openingSummary = mapper.Map<OpeningPositionsResource, OpeningPositionsSummary>(openingRes);
                RequestUnassign response = new RequestUnassign{
                                                                 OpeningId = position.Id,
                                                                 UserId = user.Id,
                                                                 ConfirmedUtilization = user.Utilization,
                                                                 Opening = openingSummary                                               
                                                                };


                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception err)
            {
                var errMessage = $"Source: {err.Source}\n  Message: {err.Message}\n  StackTrace: {err.StackTrace}\n";
                if (err is SqlException)
                {
                    var error = new InternalServerException(errMessage);
                    return StatusCode(StatusCodes.Status500InternalServerError, new CustomException<InternalServerException>(error).GetException());
                }
                else
                {
                    var error = new BadRequestException(errMessage);
                    return StatusCode(StatusCodes.Status400BadRequest, new CustomException<BadRequestException>(error).GetException());
                }
            }
        }
    }

}
