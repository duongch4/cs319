using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    public class MasterController : ControllerBase
    {
        private readonly ILocationsRepository locationsRepository;
        private readonly IDisciplinesRepository disciplinesRepository;
        private readonly IResourceDisciplineRepository resourceDisciplineRepository;
        private readonly IMapper mapper;

        public MasterController(
            ILocationsRepository locationsRepository, IDisciplinesRepository disciplinesRepository,
            IResourceDisciplineRepository resourceDisciplineRepository, IMapper mapper
        )
        {
            this.locationsRepository = locationsRepository;
            this.disciplinesRepository = disciplinesRepository;
            this.resourceDisciplineRepository = resourceDisciplineRepository;
            this.mapper = mapper;
        }

        /// <summary>Get masterlists</summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/masterlists
        ///
        /// </remarks>
        /// <returns>All available disciplines, locations, and years of exp</returns>
        /// <response code="200">Returns all available locations</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized Request</response>
        /// <response code="404">If no locations are found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("masterlists")]
        [ProducesResponseType(typeof(OkResponse<MasterResource>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedException), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMasterlists()
        {
            try
            {
                var disciplineResources = await disciplinesRepository.GetAllDisciplinesWithSkills();
                if (disciplineResources == null || !disciplineResources.Any())
                {
                    return StatusCode(StatusCodes.Status404NotFound, new NotFoundException("No disciplines data found"));
                }
                var disciplines = MapDisciplines(disciplineResources);

                var locationResources = await locationsRepository.GetAllLocationsGroupByProvince();
                if (locationResources == null || !locationResources.Any())
                {
                    return StatusCode(StatusCodes.Status404NotFound, new NotFoundException("No locations data found"));
                }
                var locations = MapLocations(locationResources);

                var yearsOfExp = await resourceDisciplineRepository.GetAllYearsOfExp();
                if (yearsOfExp == null || !yearsOfExp.Any())
                {
                    return StatusCode(StatusCodes.Status404NotFound, new NotFoundException("No yearsOfExp data found"));
                }

                var resource = new MasterResource
                {
                    Disciplines = disciplines,
                    Locations = locations,
                    YearsOfExp = yearsOfExp
                };

                var response = new OkResponse<MasterResource>(resource, "Everything is good");
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

        private Dictionary<string, MasterDiscipline> MapDisciplines(IEnumerable<DisciplineResource> disciplineResources)
        {
            char[] sep = { ',' };
            return disciplineResources.ToDictionary(
                disciplineResource => disciplineResource.Name,
                disciplineResource => new MasterDiscipline() { DisciplineID = disciplineResource.Id, Skills = disciplineResource.Skills.Split(sep) }
            );
        }

        private Dictionary<string, Dictionary<string, int>> MapLocations(IEnumerable<MasterLocation> locationResources)
        {
            char[] sep = { ',' };
            char[] innerSep = { '-' };
            return locationResources.ToDictionary<MasterLocation, string, Dictionary<string, int>>(
                locationResource => locationResource.Province,
                locationResource =>
                {
                    var pairs = locationResource.CitiesIds.Split(sep).ToDictionary(
                        pair => pair.Split(innerSep)[0],
                        pair => Int32.Parse(pair.Split(innerSep)[1])
                    );
                    return pairs;
                }
            );
        }
    }
}
