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
    //[Authorize]
    [Route("api")]
    [Produces("application/json")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectsRepository projectsRepository;
        private readonly IMapper mapper;
        private readonly IUsersRepository usersRepository;
        private readonly IPositionsRepository positionsRepository;

        public ProjectsController(IProjectsRepository projectsRepository, IMapper mapper, 
                                  IUsersRepository usersRepository, IPositionsRepository positionsRepository)
        {
            this.projectsRepository = projectsRepository;
            this.mapper = mapper;   
            this.usersRepository = usersRepository;
            this.positionsRepository = positionsRepository;
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
        [ProducesResponseType(typeof(OkResponse<IEnumerable<ProjectResource>>), StatusCodes.Status200OK)]
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
                var resource = mapper.Map<IEnumerable<Project>, IEnumerable<ProjectResource>>(projects);
                var response = new OkResponse<IEnumerable<ProjectResource>>(resource, "Everything is good");
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

        /// <summary>Get one project based on a given project number</summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/projects/2006-7H4V-72
        ///
        /// </remarks>
        /// <returns>The requested project</returns>
        /// <response code="200">Returns the requested project</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">If the requested project cannot found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("projects/{projectNumber}", Name = "GetAProject")]
        [ProducesResponseType(typeof(OkResponse<ProjectResource>), StatusCodes.Status200OK)]
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
                var project = await projectsRepository.GetAProject(projectNumber);
                if (project == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new NotFoundException($"No project at projectNumber '{projectNumber}' found"));
                }
                var resource = mapper.Map<Project, ProjectResource>(project);
                var response = new OkResponse<ProjectResource>(resource, "Everything is good");
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
        [ProducesResponseType(typeof(OkResponse<IEnumerable<ProjectResource>>), StatusCodes.Status200OK)]
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
                var resource = mapper.Map<IEnumerable<Project>, IEnumerable<ProjectResource>>(mostRecentProjects);
                var response = new OkResponse<IEnumerable<ProjectResource>>(resource, "Everything is good");
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
        [ProducesResponseType(typeof(OkResponse<ProjectResource>), StatusCodes.Status201Created)]
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
                var resource = mapper.Map<Project, ProjectResource>(created);
                var url = Url.Link(nameof(GetAProject), new { projectNumber = created.Number });
                var response = new OkResponse<ProjectResource>(resource, "Successfully created", new { url = url });
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
        [ProducesResponseType(typeof(OkResponse<ProjectResource>), StatusCodes.Status201Created)]
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
                var resource = mapper.Map<Project, ProjectResource>(updated);
                var response = new OkResponse<ProjectResource>(resource, "Successfully updated");
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

        /// <summary>Delete a project</summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/projects/2026-7H4V-72
        ///
        /// </remarks>
        /// <param name="number"></param>
        /// <returns>The old deleted project</returns>
        /// <response code="201">Returns the old deleted project</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal Server Error</response>
        [HttpDelete]
        [Route("projects/{number}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteAProject([FromRoute] string number)
        {
            if (number == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new BadRequestException("The given project number is null"));
            }

            try
            {
                var deleted = await projectsRepository.DeleteAProject(number);
                if (deleted == null) {
                    return StatusCode(StatusCodes.Status404NotFound, new NotFoundException("The given project number cannot be found on database"));
                }
                var resource = mapper.Map<Project, ProjectResource>(deleted);
                var response = new OkResponse<ProjectResource>(resource, "Successfully deleted");
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

        
        /// <summary>Assigning a Resource to a Project</summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT api/projects/2009-VD9D-15/assign/1
        ///
        /// </remarks>
        /// <param name= "reqBody">The requestBody Contents</param>
        /// <returns>The old deleted project</returns>
        /// <response code="201">Returns a RequestProjectAssign (e.g. {{positionId} {userId}})</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPut]
        [Route("projects/{projectNumber}/assign/{positionId}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> AssignAResource([FromBody] RequestProjectAssign reqBody)
        {
            try
            {
                Position position = await positionsRepository.GetAPosition(reqBody.positionId);
                if (position == null) {
                    return StatusCode(StatusCodes.Status404NotFound, new NotFoundException("The given positionId cannot be found in the database"));
                }
                position.Id = reqBody.positionId; 
                position.ResourceId = reqBody.userId;
                position = await positionsRepository.UpdateAPosition(position);
                var posIdAndResourceId = new {reqBody.positionId, reqBody.userId};
                var response = new UpdatedResponse<object>(posIdAndResourceId, "Successfully updated");
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
