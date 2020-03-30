using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.API.Application.Models;
using Web.API.Application.Repository;
using StatusCodes = Microsoft.AspNetCore.Http.StatusCodes;
using Web.API.Application.Communication;
using Web.API.Resources;
using System;
using System.Data.SqlClient;
using System.Linq;
using Serilog;

namespace Web.API.Controllers
{
    [Authorize] // TODO: Purpose for now: Auth Integration Testing on Regular User
    [Route("api")]
    [Produces("application/json")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationsRepository locationsRepository;
        private readonly IMapper mapper;
        public LocationsController(ILocationsRepository locationsRepository, IMapper mapper)
        {
            this.locationsRepository = locationsRepository;
            this.mapper = mapper;
        }

        /// <summary>Get all locations</summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/locations
        ///
        /// </remarks>
        /// <returns>All available locations</returns>
        /// <response code="200">Returns all available locations</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">If no locations are found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("locations")]
        [ProducesResponseType(typeof(OkResponse<IEnumerable<LocationResource>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllLocations()
        {
            try
            {
                var locations = await locationsRepository.GetAllLocations();
                if (locations == null || !locations.Any())
                {
                    var error = new NotFoundException("No locations data found");
                    return StatusCode(StatusCodes.Status404NotFound, new CustomException<NotFoundException>(error).GetException());
                }
                var resource = mapper.Map<IEnumerable<Location>, IEnumerable<LocationResource>>(locations);
                var response = new OkResponse<IEnumerable<LocationResource>>(resource, "Everything is good");
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

        /// <summary>Get a specific location based on a given location code</summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/locations/wpg
        ///
        /// </remarks>
        /// <param name="locationId"></param>
        /// <returns>The requested location</returns>
        /// <response code="200">Returns the requested location</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">If the requested location cannot be found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("locations/{locationId}")]
        [ProducesResponseType(typeof(OkResponse<LocationResource>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetALocation([FromRoute] int locationId)
        {
            try
            {
                var location = await locationsRepository.GetALocation(locationId);
                if (location == null)
                {
                    var error = new NotFoundException($"No location at city '{locationId}' found");
                    return StatusCode(StatusCodes.Status404NotFound, new CustomException<NotFoundException>(error).GetException());
                }
                var resource = mapper.Map<Location, LocationResource>(location);
                var response = new OkResponse<LocationResource>(resource, "Everything is good");
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
