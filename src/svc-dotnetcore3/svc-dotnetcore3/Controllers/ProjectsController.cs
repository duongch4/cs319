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
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectsRepository projectsRepository;
        private readonly IUsersRepository usersRepository;
        private readonly IPositionsRepository positionsRepository;
        private readonly ILocationsRepository locationsRepository;
        private readonly ISkillsRepository skillsRepository;
        private readonly IMapper mapper;

        public ProjectsController(
            IProjectsRepository projectsRepository, IUsersRepository usersRepository,
            IPositionsRepository positionsRepository, ILocationsRepository locationsRepository,
            ISkillsRepository skillsRepository, IMapper mapper
        )
        {
            this.projectsRepository = projectsRepository;
            this.usersRepository = usersRepository;
            this.positionsRepository = positionsRepository;
            this.locationsRepository = locationsRepository;
            this.skillsRepository = skillsRepository;
            this.mapper = mapper;
        }

        /// <summary>Get all projects</summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/projects
        ///
        /// </remarks>
        /// <returns>All available projects</returns>
        /// <response code="200">Returns all available projects</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">If no projects are found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("projects")]
        [ProducesResponseType(typeof(OkResponse<IEnumerable<ProjectProfile>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllProjects()
        {
            try
            {
                var projects = await projectsRepository.GetAllProjects();
                if (projects == null || !projects.Any())
                {
                    return StatusCode(StatusCodes.Status404NotFound, new NotFoundException("No projects data found"));
                }
                var resource = mapper.Map<IEnumerable<Project>, IEnumerable<ProjectProfile>>(projects);
                var response = new OkResponse<IEnumerable<ProjectProfile>>(resource, "Everything is good");
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

        /// <summary>Get one project</summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/projects/2005-KJS4-46
        ///
        /// </remarks>
        /// <param name="projectNumber"></param>
        /// <returns>The requested project</returns>
        /// <response code="200">Returns the requested project</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">If the requested project cannot found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("projects/{projectNumber}", Name = "GetAProject")]
        [ProducesResponseType(typeof(OkResponse<ProjectProfile>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAProject(string projectNumber)
        {
            if (projectNumber == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new BadRequestException("The given project number is null"));
            }
            try
            {
                var project = await projectsRepository.GetAProjectResource(projectNumber);
                if (project == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new NotFoundException($"No project at projectNumber '{projectNumber}' found"));
                }
                var projectSummary = mapper.Map<ProjectResource, ProjectSummary>(project);

                var users = await usersRepository.GetAllUsersResourceOnProject(project.Id);
                if (users == null || !users.Any())
                {
                    return StatusCode(StatusCodes.Status404NotFound, new NotFoundException($"No User at projectNumber '{projectNumber}' found"));
                }
                var usersSummary = mapper.Map<IEnumerable<UserResource>, IEnumerable<UserSummary>>(users);

                var openingPositions = await positionsRepository.GetAllUnassignedPositionsResourceOfProject(project.Id);
                if (openingPositions == null || !openingPositions.Any())
                {
                    return StatusCode(StatusCodes.Status404NotFound, new NotFoundException($"No Opening Positions at projectNumber '{projectNumber}' found"));
                }
                var openingPositionsSummary = mapper.Map<IEnumerable<OpeningPositionsResource>, IEnumerable<OpeningPositionsSummary>>(openingPositions);

                var projectProfile = new ProjectProfile {
                    ProjectSummary = projectSummary,
                    UsersSummary = usersSummary,
                    Openings = openingPositionsSummary
                };

                var response = new OkResponse<ProjectProfile>(projectProfile, "Everything is good");
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

        /// <summary>Get the most recent projects</summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/projects/most-recent
        ///
        /// </remarks>
        /// <returns>The requested project</returns>
        /// <response code="200">Returns the most recent projects</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">If no projects are found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("projects/most-recent")]
        [ProducesResponseType(typeof(OkResponse<IEnumerable<ProjectProfile>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMostRecentProjects()
        {
            try
            {
                var mostRecentProjects = await projectsRepository.GetMostRecentProjects();
                if (mostRecentProjects == null || !mostRecentProjects.Any())
                {
                    return StatusCode(StatusCodes.Status404NotFound, new NotFoundException($"No projects found"));
                }
                var resource = mapper.Map<IEnumerable<Project>, IEnumerable<ProjectProfile>>(mostRecentProjects);
                var response = new OkResponse<IEnumerable<ProjectProfile>>(resource, "Everything is good");
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

        /// <summary>Create a project</summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/projects
        ///     {
        ///        "Id": 9999,
        ///        "Number": "2026-7H4V-72",
        ///        "Title": "Deserunt eum earum neque voluptatem.",
        ///        "LocationId": 81
        ///     }
        ///
        /// </remarks>
        /// <param name="project"></param>
        /// <returns>A newly created project</returns>
        /// <response code="201">Returns the newly created project</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPost]
        [Route("projects")]
        [ProducesResponseType(typeof(CreatedResponse<ProjectProfile>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAProject([FromBody] Project project)
        {
            if (project == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new BadRequestException("The given project is null / Request Body cannot be read"));
            }

            try
            {
                Log.Logger.Here().Information("{@Project}", project);
                var created = await projectsRepository.CreateAProject(project);
                var resource = mapper.Map<Project, ProjectProfile>(created);
                var url = Url.Link(nameof(GetAProject), new { projectNumber = created.Number });
                var response = new CreatedResponse<ProjectProfile>(resource, "Successfully created", new { url = url });
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

        /// <summary>Update a project</summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/projects
        ///     {
        ///        "Id": 9999,
        ///        "Number": "2050-7H4V-72",
        ///        "Title": "Updated title",
        ///        "LocationId": 81
        ///     }
        ///
        /// </remarks>
        /// <param name="project"></param>
        /// <returns>The newly updated project</returns>
        /// <response code="201">Returns the newly updated project</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPut]
        [Route("projects")]
        [ProducesResponseType(typeof(UpdatedResponse<ProjectProfile>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateAProject([FromBody] Project project)
        {
            if (project == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new BadRequestException("The given project is null / Request Body cannot be read"));
            }

            try
            {
                Log.Logger.Here().Information("{@Project}", project);
                var updated = await projectsRepository.UpdateAProject(project);
                var resource = mapper.Map<Project, ProjectProfile>(updated);
                var response = new UpdatedResponse<ProjectProfile>(resource, "Successfully updated");
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

        // /// <summary>Assigns a user to an opening and modifies the project</summary>
        // /// <remarks>
        // /// Sample request:
        // ///
        // ///     PUT /api/projects/{projectId}/assign/{openingId}
        // ///     {
        // ///        "PositionId": 1,
        // ///        "UserId": 2
        // ///     }
        // ///
        // /// </remarks>
        // /// <param name="projectId"></param>
        // /// <param name="openingId"></param>
        // /// <param name="req"></param>
        // /// <returns>The newly updated project</returns>
        // /// <response code="201">Returns the newly updated project</response>
        // /// <response code="400">Bad Request</response>
        // /// <response code="500">Internal Server Error</response>
        // [HttpPut]
        // [Route("projects/{projectId}/assign/{openingId}")]
        // [ProducesResponseType(typeof(OkResponse<ProjectProfile>), StatusCodes.Status201Created)]
        // [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        // [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        // public async Task<IActionResult> UpdateAProject(int projectId, int openingId, [FromBody] RequestProjectAssign req)
        // {
        //     Log.Information("{@a}", req);
        //     Log.Information("{@a}", projectId);
        //     Log.Information("{@a}", openingId);
        //     // Log.Information("{@a}", openingId);

        //     if (req == null)
        //     {
        //         return StatusCode(StatusCodes.Status400BadRequest, new BadRequestException("The given Request Body cannot be read"));
        //     }

        //     try
        //     {
        //         // Log.Logger.Here().Information("{@Req}", req);
        //         // User user = new {
        //         //     Id = req.userId,
        //         //     FirstName 
        //         // };
        //         // var assginedUser = await usersRepository.UpdateAUser();
        //         // var updated = await projectsRepository.UpdateAProject(project);
        //         // var resource = mapper.Map<Project, ProjectResource>(updated);
        //         // var response = new OkResponse<ProjectResource>(resource, "Successfully updated");
        //         // return StatusCode(StatusCodes.Status200OK, response);
        //         return Ok(req);
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

        /// <summary>Delete a project</summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/projects/2005-KJS4-46
        ///
        /// </remarks>
        /// <param name="projectNumber"></param>
        /// <returns>The old deleted project</returns>
        /// <response code="201">Returns the old deleted project</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">If no projects are found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpDelete]
        [Route("projects/{number}")]
        [ProducesResponseType(typeof(DeletedResponse<ProjectProfile>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAProject([FromRoute] string projectNumber)
        {
            if (projectNumber == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new BadRequestException("The given project number is null"));
            }

            try
            {
                var deleted = await projectsRepository.DeleteAProject(projectNumber);
                if (deleted == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new NotFoundException("The given project number cannot be found on database"));
                }
                var resource = mapper.Map<Project, ProjectProfile>(deleted);
                var response = new DeletedResponse<ProjectProfile>(resource, "Successfully deleted");
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
    public class OldProjectsController : ControllerBase
    {
        private readonly IProjectsRepository projectsRepository;
        private readonly IMapper mapper;

        public OldProjectsController(IProjectsRepository projectsRepository, IMapper mapper)
        {
            this.projectsRepository = projectsRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        [Route("/projects")]
        public async Task<IActionResult> GetAllProjects()
        {
            var response = await projectsRepository.GetAllProjects();
            var viewModel = mapper.Map<IEnumerable<Project>>(response);
            return Ok(viewModel);
        }
    }
}
