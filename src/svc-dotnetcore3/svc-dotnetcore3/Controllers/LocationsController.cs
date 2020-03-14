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
    [Authorize]
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
                    return StatusCode(StatusCodes.Status404NotFound, new NotFoundException("No locations data found"));
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
                    return StatusCode(StatusCodes.Status500InternalServerError, error);
                }
                else
                {
                    var error = new BadRequestException(errMessage);
                    return StatusCode(StatusCodes.Status400BadRequest, error);
                }
            }
        }

        // /// <summary>Get a specific location based on a given location code</summary>
        // /// <remarks>
        // /// Sample request:
        // ///
        // ///     GET /api/locations/wpg
        // ///
        // /// </remarks>
        // /// <param name="city"></param>
        // /// <returns>The requested location</returns>
        // /// <response code="200">Returns the requested location</response>
        // /// <response code="400">Bad Request</response>
        // /// <response code="404">If the requested location cannot be found</response>
        // /// <response code="500">Internal Server Error</response>
        // [HttpGet]
        // [Route("locations/{city}")]
        // [ProducesResponseType(typeof(OkResponse<LocationResource>), StatusCodes.Status200OK)]
        // [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        // [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        // [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]
        // public async Task<IActionResult> GetALocationWithCity([FromRoute] string city)
        // {
        //     if (city == null)
        //     {
        //         return StatusCode(StatusCodes.Status400BadRequest, new BadRequestException("The given city is null"));
        //     }
        //     try
        //     {
        //         var location = await locationsRepository.GetALocationWithCity(city);
        //         if (location == null)
        //         {
        //             return StatusCode(StatusCodes.Status404NotFound, new NotFoundException($"No location at city '{city}' found"));
        //         }
        //         var resource = mapper.Map<Location, LocationResource>(location);
        //         var response = new OkResponse<LocationResource>(resource, "Everything is good");
        //         return StatusCode(StatusCodes.Status200OK, response);
        //     }
        //     catch (Exception err)
        //     {
        //         var errMessage = $"Source: {err.Source}\n  Message: {err.Message}\n  StackTrace: {err.StackTrace}\n";
        //         if (err is SqlException)
        //         {
        //             var error = new InternalServerException(errMessage);
        //             return StatusCode(StatusCodes.Status500InternalServerError, error);
        //         }
        //         else
        //         {
        //             var error = new BadRequestException(errMessage);
        //             return StatusCode(StatusCodes.Status400BadRequest, error);
        //         }
        //     }
        // }
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
                    return StatusCode(StatusCodes.Status404NotFound, new NotFoundException($"No location at city '{locationId}' found"));
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
    

    [Authorize]
    public class OldLocationsController : ControllerBase
    {
        private readonly ILocationsRepository locationsRepository;
        private readonly IMapper mapper;

        public OldLocationsController(ILocationsRepository locationsRepository, IMapper mapper)
        {
            this.locationsRepository = locationsRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        [Route("/locations")]
        public async Task<ActionResult<IEnumerable<Location>>> GetAllLocations()
        {
            var response = await locationsRepository.GetAllLocations();
            var viewModel = mapper.Map<IEnumerable<Location>>(response);
            return Ok(viewModel);
        }
    }
}
