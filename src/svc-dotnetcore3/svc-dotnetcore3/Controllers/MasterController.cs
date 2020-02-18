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
    public class MasterController : ControllerBase
    {
        private readonly IProjectsRepository projectsRepository;
        private readonly IUsersRepository usersRepository;
        private readonly IPositionsRepository positionsRepository;
        private readonly ILocationsRepository locationsRepository;
        private readonly IDisciplinesRepository disciplinesRepository;
        private readonly ISkillsRepository skillsRepository;
        private readonly IResourceDisciplineRepository resourceDisciplineRepository;
        private readonly IMapper mapper;

        public MasterController(
            IProjectsRepository projectsRepository, IUsersRepository usersRepository,
            IPositionsRepository positionsRepository, ILocationsRepository locationsRepository,
            IDisciplinesRepository disciplinesRepository, ISkillsRepository skillsRepository,
            IResourceDisciplineRepository resourceDisciplineRepository, IMapper mapper
        )
        {
            this.projectsRepository = projectsRepository;
            this.usersRepository = usersRepository;
            this.positionsRepository = positionsRepository;
            this.locationsRepository = locationsRepository;
            this.disciplinesRepository = disciplinesRepository;
            this.skillsRepository = skillsRepository;
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
        /// <response code="404">If no locations are found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("masterlists")]
        [ProducesResponseType(typeof(OkResponse<MasterResource>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMasterlists()
        {
            try
            {
                var disciplines = await disciplinesRepository.GetAllDisciplines();
                if (disciplines == null || !disciplines.Any())
                {
                    return StatusCode(StatusCodes.Status404NotFound, new NotFoundException("No disciplines data found"));
                }

                Dictionary<string, string[]> disciplinesResource = new Dictionary<string, string[]>();
                foreach (var discipline in disciplines)
                {
                    var skills = await skillsRepository.GetSkillsWithDiscipline(discipline.Name);
                    var skillsResource = mapper.Map<IEnumerable<Skill>, IEnumerable<SkillResource>>(skills);
                    disciplinesResource.Add(discipline.Name, skillsResource.Select(skillsResource => skillsResource.Name).ToArray());
                }

                var locations = locationsRepository.GetStaticLocations();
                var yearsOfExp = await resourceDisciplineRepository.GetAllYearsOfExp();
                if (yearsOfExp == null || !yearsOfExp.Any())
                {
                    return StatusCode(StatusCodes.Status404NotFound, new NotFoundException("No yearsOfExp data found"));
                }

                var resource = new MasterResource
                {
                    disciplines = disciplinesResource,
                    locations = locations,
                    yearsOfExp = yearsOfExp
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
    }
}
