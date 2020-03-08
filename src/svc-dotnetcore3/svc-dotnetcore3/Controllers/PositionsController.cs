using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StatusCodes = Microsoft.AspNetCore.Http.StatusCodes;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.API.Application.Models;
using Web.API.Application.Repository;
using Web.API.Application.Communication;
using Web.API.Resources;

using System;
using System.Data.SqlClient;
using System.Linq;
using Serilog;

namespace Web.API.Controllers
{
    [Authorize]
    [Route("api")]
    [Produces("application/json")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class PositionsController : ControllerBase
    {
        private readonly IProjectsRepository projectsRepository;
        private readonly IUsersRepository usersRepository;
        private readonly IPositionsRepository positionsRepository;

        private readonly IUtilizationRepository utilizationRepository;
        private readonly IMapper mapper;

        public PositionsController(
            IProjectsRepository projectsRepository, IUsersRepository usersRepository,
            IPositionsRepository positionsRepository, IUtilizationRepository utilizationRepository,
            IMapper mapper
        )
        {
            this.projectsRepository = projectsRepository;
            this.usersRepository = usersRepository;
            this.positionsRepository = positionsRepository;
            this.utilizationRepository = utilizationRepository;
            this.mapper = mapper;
        }

        /// <summary>Assigning a Resource to a Project</summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT api/projects/2009-VD9D-15/assign/1
        ///
        /// </remarks>
        /// <param name= "openingId">The id of the opening the resource will be assigned to</param>
        /// <param name = "userId"> The id of the resource being assigned to the opening </param>
        /// <returns>The old deleted project</returns>
        /// <response code="201">Returns a RequestProjectAssign (e.g. {{positionId} {userId}})</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized Request</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPut]
        [Route("/positions/{openingId}/assign/{userId}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedException), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AssignAResource([FromRoute] int openingId, int userId)
        {
            try
            {
                Position position = await positionsRepository.GetAPosition(openingId);
                User user = await usersRepository.GetAUser(userId);
                if (position == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new NotFoundException("The given position does not exist."));
                }
                if (user == null) {
                    return StatusCode(StatusCodes.Status404NotFound, new NotFoundException("The given resource was not found"));
                }

                position.Id = openingId;
                position.ResourceId = userId;
                position = await positionsRepository.UpdateAPosition(position);

                var resourceUtilizations = await utilizationRepository.GetUtilizationOfUser(userId);
                var responseObject = new RequestProjectAssign();
                responseObject.OpeningId = openingId;
                responseObject.UserID = userId;
                
                foreach(rawUtilization utilization in resourceUtilizations) {
                    if (utilization.isConfirmed) {
                        responseObject.ConfirmedUtilization = utilization.utilization;
                    }
                }
                var response = new UpdatedResponse<RequestProjectAssign>(responseObject, "Successfully updated");
                return StatusCode(StatusCodes.Status201Created, response);
            }
            catch (Exception err)
            {
                var errMessage = $"Source: {err.Source}\n  Message: {err.Message}\n  StackTrace: {err.StackTrace}\n";
                if (err is SqlException)
                {
                    var error = new InternalServerException(errMessage);
                    return StatusCode(StatusCodes.Status500InternalServerError, error);
                }
                else
                {
                    var error = new BadRequestException(errMessage);
                    return StatusCode(StatusCodes.Status400BadRequest, error);
                }
            }
        }
    }


}
