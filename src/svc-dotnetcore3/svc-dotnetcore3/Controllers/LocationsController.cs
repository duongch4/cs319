using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.API.Application.Models;
using Web.API.Application.Repository;
using StatusCodes = Microsoft.AspNetCore.Http.StatusCodes;
using Web.API.Resources;

using System;
using Newtonsoft.Json;

namespace Web.API.Controllers
{
    [Authorize]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationsRepository locationsRepository;
        private readonly IMapper mapper;

        public LocationsController(ILocationsRepository locationsRepository, IMapper mapper)
        {
            this.locationsRepository = locationsRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        [Route("/locations")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllLocations()
        {
            var locations = await locationsRepository.GetAllLocations();
            var resource = mapper.Map<IEnumerable<Location>>(locations);
            var response = new Response(resource, StatusCodes.Status200OK, "OK", "Everything is good");
            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpGet]
        [Route("locations/{locationCode}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetALocation(string locationCode)
        {
            var location = await locationsRepository.GetALocation(locationCode);
            var resource = mapper.Map<Location>(response);
            var response = new Response(resource, StatusCodes.Status200OK, "OK", "Everything is good");
            return StatusCode(StatusCodes.Status200OK, response);
        }
    }
}