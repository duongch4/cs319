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

        /// <summary>Get projects for a specific page number</summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/projects?orderKey={orderKey}&#38;order={order}&#38;page={pageNumber}
        ///
        /// </remarks>
        /// <param name="searchWord" />
        /// <param name="orderKey" />
        /// <param name="order" />
        /// <param name="page" />
        /// <returns>Payload: List of ProjectSummary</returns>
        /// <response code="200">
        ///     Returns at most the top 50 projects that match the provided keyValue.
        ///     When no key value is provided, it returns a list of the top 50 projects whose end dates are after the current date.
        ///     The projects are sorted according to their start dates.
        /// </response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized Request</response>
        /// <response code="404">If no projects are found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("projects")]
        [ProducesResponseType(typeof(OkResponse<IEnumerable<ProjectSummary>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedException), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllProjects([FromQuery] string searchWord, [FromQuery] string orderKey, [FromQuery] string order, [FromQuery] int page)
        {
            orderKey = (orderKey == null) ? "startDate" : orderKey;
            order = (order == null) ? "asc" : order;
            page = (page == 0) ? 1 : page;
            try
            {
                IEnumerable<ProjectResource> projects;
                if (searchWord == null)
                {
                    projects = await projectsRepository.GetAllProjectResources(orderKey, order, page);
                }
                else
                {
                    projects = await projectsRepository.GetAllProjectResourcesWithTitle(searchWord, orderKey, order, page);
                }

                if (projects == null || !projects.Any())
                {
                    return StatusCode(StatusCodes.Status404NotFound, new NotFoundException("No projects data found"));
                }
                var resource = mapper.Map<IEnumerable<ProjectResource>, IEnumerable<ProjectSummary>>(projects);
                var extra = new
                {
                    searchWord = searchWord,
                    page = page,
                    size = resource.Count(),
                    order = order,
                    orderKey = orderKey
                };
                var response = new OkResponse<IEnumerable<ProjectSummary>>(resource, "Everything is good", extra);
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
        ///     GET /api/projects/2009-VD9D-15
        ///
        /// </remarks>
        /// <param name="projectNumber"></param>
        /// <returns>The requested project</returns>
        /// <response code="200">Returns the requested project</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized Request</response>
        /// <response code="404">If the requested project cannot be found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("projects/{projectNumber}", Name = "GetAProject")]
        [ProducesResponseType(typeof(OkResponse<ProjectProfile>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedException), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
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

                var projectManager = mapper.Map<ProjectResource, ProjectManager>(project);

                var users = await usersRepository.GetAllUserResourcesOnProject(project.Id, project.ManagerId);
                if (users == null || !users.Any())
                {
                    users = new UserResource[] { };
                }
                var usersSummary = mapper.Map<IEnumerable<UserResource>, IEnumerable<UserSummary>>(users);

                var openingPositions = await positionsRepository.GetAllUnassignedPositionsResourceOfProject(project.Id);
                if (openingPositions == null || !openingPositions.Any())
                {
                    // return StatusCode(StatusCodes.Status404NotFound, new NotFoundException($"No Opening Positions at projectNumber '{projectNumber}' found"));
                    openingPositions = new OpeningPositionsResource[] { };
                }
                var openingPositionsSummary = mapper.Map<IEnumerable<OpeningPositionsResource>, IEnumerable<OpeningPositionsSummary>>(openingPositions);

                var projectProfile = new ProjectProfile
                {
                    ProjectSummary = projectSummary,
                    ProjectManager = projectManager,
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
        /// <response code="401">Unauthorized Request</response>
        /// <response code="404">If no projects are found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("projects/most-recent")]
        [ProducesResponseType(typeof(OkResponse<IEnumerable<ProjectProfile>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedException), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
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
        ///         "projectSummary": {
        ///             "title": "POST Title",
        ///             "location": {
        ///                 "province": "test Province",
        ///                 "city": "Vancouver"
        ///             },
        ///             "projectStartDate": "2020-10-31T00:00:00.0000000",
        ///             "projectEndDate": "2021-02-12T00:00:00.0000000",
        ///             "projectNumber": "0000-0000-00"
        ///         },
        ///         "projectManager": {
        ///             "userID": 5,
        ///             "lastName": "Lulu",
        ///             "firstName": "Lala"
        ///         },
        ///         "usersSummary": [
        ///             {
        ///                 "firstName": "test FirstName 1",
        ///                 "lastName": "test LastName 1",
        ///                 "userID": 1,
        ///                 "location": {
        ///                     "province": "test Province User 1",
        ///                     "city": "test City User 1"
        ///                 },
        ///                 "utilization": 100,
        ///                 "resourceDiscipline": {
        ///                     "discipline": "test d1",
        ///                     "yearsOfExp": "3-5"
        ///                 },
        ///                 "isConfirmed": true
        ///             },
        ///             {
        ///                 "firstName": "test FirstName 2",
        ///                 "lastName": "test LastName 2",
        ///                 "userID": 2,
        ///                 "location": {
        ///                     "province": "test Province User 2",
        ///                     "city": "test City User 2"
        ///                 },
        ///                 "utilization": 90,
        ///                 "resourceDiscipline": {
        ///                     "discipline": "test d1",
        ///                     "yearsOfExp": "3-5"
        ///                 },
        ///                 "isConfirmed": false
        ///             }
        ///         ],
        ///         "openings": [
        ///             {
        ///                 "discipline": "Weapons",
        ///                 "skills": ["Glock", "Sniper Rifle"],
        ///                 "yearsOfExp": "1-3",
        ///                 "commitmentMonthlyHours": 160
        ///             },
        ///             {
        ///                 "discipline": "Intel",
        ///                 "skills": ["Deception", "False Identity Creation"],
        ///                 "yearsOfExp": "3-5",
        ///                 "commitmentMonthlyHours": 180
        ///             }
        ///         ]
        ///     }
        ///
        /// </remarks>
        /// <param name="projectProfile"></param>
        /// <returns>A newly created project</returns>
        /// <response code="201">Returns the newly created project</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized Request</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPost]
        [Route("projects")]
        [ProducesResponseType(typeof(CreatedResponse<string>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedException), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateAProject([FromBody] ProjectProfile projectProfile)
        {
            if (projectProfile == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new BadRequestException("The given project is null / Request Body cannot be read"));
            }

            try
            {
                var location = await locationsRepository.GetALocation(projectProfile.ProjectSummary.Location.City);
                var createdProjectNumber = await projectsRepository.CreateAProject(projectProfile, location.Id);
                var response = new CreatedResponse<string>(createdProjectNumber, $"Successfully created project number '{createdProjectNumber}'");
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
        ///     PUT /api/projects/0000-0000-00
        ///     {
        ///         "projectSummary": {
        ///             "title": "POST Title",
        ///             "location": {
        ///                 "province": "test Province",
        ///                 "city": "Vancouver"
        ///             },
        ///             "projectStartDate": "2020-10-31T00:00:00.0000000",
        ///             "projectEndDate": "2021-02-12T00:00:00.0000000",
        ///             "projectNumber": "0000-0000-00"
        ///         },
        ///         "projectManager": {
        ///             "userID": 5,
        ///             "lastName": "Lulu",
        ///             "firstName": "Lala"
        ///         },
        ///         "usersSummary": [
        ///             {
        ///                 "firstName": "test FirstName 1",
        ///                 "lastName": "test LastName 1",
        ///                 "userID": 1,
        ///                 "location": {
        ///                     "province": "test Province User 1",
        ///                     "city": "test City User 1"
        ///                 },
        ///                 "utilization": 100,
        ///                 "resourceDiscipline": {
        ///                     "discipline": "test d1",
        ///                     "yearsOfExp": "3-5"
        ///                 },
        ///                 "isConfirmed": true
        ///             },
        ///             {
        ///                 "firstName": "test FirstName 2",
        ///                 "lastName": "test LastName 2",
        ///                 "userID": 2,
        ///                 "location": {
        ///                     "province": "test Province User 2",
        ///                     "city": "test City User 2"
        ///                 },
        ///                 "utilization": 90,
        ///                 "resourceDiscipline": {
        ///                     "discipline": "test d1",
        ///                     "yearsOfExp": "3-5"
        ///                 },
        ///                 "isConfirmed": false
        ///             }
        ///         ],
        ///         "openings": [
        ///             {
        ///                 "discipline": "Weapons",
        ///                 "skills": ["Glock", "Sniper Rifle"],
        ///                 "yearsOfExp": "1-3",
        ///                 "commitmentMonthlyHours": 160
        ///             },
        ///             {
        ///                 "discipline": "Intel",
        ///                 "skills": ["Deception", "False Identity Creation"],
        ///                 "yearsOfExp": "3-5",
        ///                 "commitmentMonthlyHours": 180
        ///             }
        ///         ]
        ///     }
        ///
        /// </remarks>
        /// <param name="projectProfile"></param>
        /// <param name="projectNumber"></param>
        /// <returns>An updated project</returns>
        /// <response code="200">Returns the updated project</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized Request</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPut]
        [Route("projects/{projectNumber}")]
        [ProducesResponseType(typeof(UpdatedResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedException), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAProject([FromBody] ProjectProfile projectProfile, string projectNumber)
        {
            if (projectProfile == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new BadRequestException("The given project profile is null / Request Body cannot be read"));
            }

            try
            {
                // Log.Logger.Here().Information("{@Project}", projectProfile);
                var location = await locationsRepository.GetALocation(projectProfile.ProjectSummary.Location.City);
                var updated = await projectsRepository.UpdateAProject(projectProfile, location.Id);
                if (updated == null)
                {
                    var errMessage = $"Query returns failure status on updating project number '{projectProfile.ProjectSummary.ProjectNumber}'";
                    return StatusCode(StatusCodes.Status500InternalServerError, new InternalServerException(errMessage));
                }
                var response = new UpdatedResponse<string>(updated, "Successfully updated");
                return StatusCode(StatusCodes.Status200OK, response);
                // return Ok(projectProfile);
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
        ///     DELETE /api/projects/0000-0000-00
        ///
        /// </remarks>
        /// <param name="projectNumber"></param>
        /// <returns>The old deleted project number</returns>
        /// <response code="200">Returns the old deleted project number</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized Request</response>
        /// <response code="404">If no projects are found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpDelete]
        [Route("projects/{projectNumber}")]
        [ProducesResponseType(typeof(DeletedResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedException), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
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
                var response = new DeletedResponse<string>(deleted.Number, $"Successfully deleted project with number '{deleted.Number}'");
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
        /// <param name= "reqBody">The requestBody</param>
        /// <returns>The old deleted project</returns>
        /// <response code="201">Returns a RequestProjectAssign (e.g. {{positionId} {userId}})</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized Request</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPut]
        [Route("projects/{projectNumber}/assign/{positionId}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedException), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AssignAResource([FromBody] RequestProjectAssign reqBody)
        {
            try
            {
                Position position = await positionsRepository.GetAPosition(reqBody.PositionID);
                if (position == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new NotFoundException("The given positionId cannot be found in the database"));
                }
                position.Id = reqBody.PositionID;
                position.ResourceId = reqBody.UserID;

                position = await positionsRepository.UpdateAPosition(position);
                var posIdAndResourceId = new { reqBody.PositionID, reqBody.UserID };
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

    // [Authorize]
    // public class OldProjectsController : ControllerBase
    // {
    //     private readonly IProjectsRepository projectsRepository;
    //     private readonly IMapper mapper;

    //     public OldProjectsController(IProjectsRepository projectsRepository, IMapper mapper)
    //     {
    //         this.projectsRepository = projectsRepository;
    //         this.mapper = mapper;
    //     }

    //     [HttpGet]
    //     [Route("/projects")]
    //     public async Task<IActionResult> GetAllProjects()
    //     {
    //         var response = await projectsRepository.GetAllProjects();
    //         var viewModel = mapper.Map<IEnumerable<Project>>(response);
    //         return Ok(viewModel);
    //     }
    // }
}
