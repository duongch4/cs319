using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.API.Application.Repository;
using StatusCodes = Microsoft.AspNetCore.Http.StatusCodes;
using Web.API.Application.Communication;
using Web.API.Resources;
using Web.API.Authorization;

using System;
using System.Data.SqlClient;
using System.Linq;
using Serilog;

namespace Web.API.Controllers
{
    // [Authorize]
    [Authorize(Actions.AdminThings)]
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
        /// <response code="403">Forbidden Request</response>
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
                    var error = new NotFoundException("No disciplines data found");
                    return StatusCode(StatusCodes.Status404NotFound, new CustomException<NotFoundException>(error).GetException());
                }
                var disciplines = MapDisciplines(disciplineResources);

                var locationResources = await locationsRepository.GetAllLocationsGroupByProvince();
                if (locationResources == null || !locationResources.Any())
                {
                    var error = new NotFoundException("No locations data found");
                    return StatusCode(StatusCodes.Status404NotFound, new CustomException<NotFoundException>(error).GetException());
                }
                var locations = MapLocations(locationResources);

                var yearsOfExp = await resourceDisciplineRepository.GetAllYearsOfExp();
                if (yearsOfExp == null || !yearsOfExp.Any())
                {
                    var error = new NotFoundException("No yearsOfExp data found");
                    return StatusCode(StatusCodes.Status404NotFound, new CustomException<NotFoundException>(error).GetException());
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
                    return StatusCode(StatusCodes.Status500InternalServerError, new CustomException<InternalServerException>(error).GetException());
                }
                else
                {
                    var error = new BadRequestException(errMessage);
                    return StatusCode(StatusCodes.Status400BadRequest, new CustomException<BadRequestException>(error).GetException());
                }
            }
        }

        private Dictionary<string, MasterDiscipline> MapDisciplines(IEnumerable<DisciplineResource> disciplineResources)
        {
            char[] sep = { ',' };
            return disciplineResources.ToDictionary(
                disciplineResource => disciplineResource.Name,
                disciplineResource =>
                {
                    IEnumerable<string> skills;
                    if (String.IsNullOrEmpty(disciplineResource.Skills))
                    {
                        skills = Enumerable.Empty<string>();
                    }
                    else
                    {
                        skills = disciplineResource.Skills.Split(sep);
                    }
                    return new MasterDiscipline()
                    {
                        DisciplineID = disciplineResource.Id,
                        Skills = skills
                    };
                }
            );
        }

        private Dictionary<string, Dictionary<string, int>> MapLocations(IEnumerable<MasterLocation> locationResources)
        {
            var sepsMap = new
            {
                sep = ',',
                innerSep = '#'
            };
            return locationResources.ToDictionary<MasterLocation, string, Dictionary<string, int>>(
                locationResource => locationResource.Province,
                locationResource =>
                {
                    Dictionary<string, int> pairs;
                    if (locationResource.CitiesIds.Equals(sepsMap.innerSep.ToString()))
                    {
                        pairs = new Dictionary<string, int>();
                    }
                    else
                    {
                        pairs = locationResource.CitiesIds.Split(sepsMap.sep).ToDictionary(
                            pair => pair.Split(sepsMap.innerSep)[0],
                            pair => Int32.Parse(pair.Split(sepsMap.innerSep)[1])
                        );
                    }
                    return pairs;
                }
            );
        }
    }
}
