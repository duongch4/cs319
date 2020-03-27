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
using Web.API.Authorization;

using System;
using System.Data.SqlClient;
using System.Linq;
using Serilog;

namespace Web.API.Controllers
{
    [Authorize(Actions.AdminThings)]
    // [Authorize(Actions.AdminThings)]
    [Route("api")]
    [Produces("application/json")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectsRepository projectsRepository;
        private readonly IUsersRepository usersRepository;
        private readonly IPositionsRepository positionsRepository;
        private readonly ILocationsRepository locationsRepository;
        private readonly IMapper mapper;

        public ProjectsController(
            IProjectsRepository projectsRepository, IUsersRepository usersRepository,
            IPositionsRepository positionsRepository, ILocationsRepository locationsRepository,
            IMapper mapper
        )
        {
            this.projectsRepository = projectsRepository;
            this.usersRepository = usersRepository;
            this.positionsRepository = positionsRepository;
            this.locationsRepository = locationsRepository;
            this.mapper = mapper;
        }

        /// <summary>Get projects with optional query string</summary>
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
                if (String.IsNullOrEmpty(searchWord))
                {
                    projects = await projectsRepository.GetAllProjectResources(orderKey, order, page);
                }
                else
                {
                    projects = await projectsRepository.GetAllProjectResourcesWithTitle(searchWord, orderKey, order, page);
                }

                if (projects == null || !projects.Any())
                {
                    var error = new NotFoundException("No projects data found");
                    return StatusCode(StatusCodes.Status404NotFound, new CustomException<NotFoundException>(error).GetException());
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
                    return StatusCode(StatusCodes.Status500InternalServerError, new CustomException<InternalServerException>(error).GetException());
                }
                else
                {
                    var error = new BadRequestException(errMessage);
                    return StatusCode(StatusCodes.Status400BadRequest, new CustomException<BadRequestException>(error).GetException());
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
                var error = new BadRequestException("The given project number is null");
                return StatusCode(StatusCodes.Status400BadRequest, new CustomException<BadRequestException>(error).GetException());
            }
            try
            {
                var project = await projectsRepository.GetAProjectResource(projectNumber);
                if (project == null)
                {
                    var error = new NotFoundException($"No project at projectNumber '{projectNumber}' found");
                    return StatusCode(StatusCodes.Status404NotFound, new CustomException<NotFoundException>(error).GetException());
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
                    return StatusCode(StatusCodes.Status500InternalServerError, new CustomException<InternalServerException>(error).GetException());
                }
                else
                {
                    var error = new BadRequestException(errMessage);
                    return StatusCode(StatusCodes.Status400BadRequest, new CustomException<BadRequestException>(error).GetException());
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
                    var error = new NotFoundException($"No projects found");
                    return StatusCode(StatusCodes.Status404NotFound, new CustomException<NotFoundException>(error).GetException());
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
                    return StatusCode(StatusCodes.Status500InternalServerError, new CustomException<InternalServerException>(error).GetException());
                }
                else
                {
                    var error = new BadRequestException(errMessage);
                    return StatusCode(StatusCodes.Status400BadRequest, new CustomException<BadRequestException>(error).GetException());
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
        ///                 "locationID": 14,
        ///                 "province": "Alberta",
        ///                 "city": "Lethbridge"
        ///             },
        ///             "projectStartDate": "2020-10-31T00:00:00.0000000",
        ///             "projectEndDate": "2021-02-12T00:00:00.0000000",
        ///             "projectNumber": "0000-0000-05"
        ///         },
        ///         "projectManager": {
        ///             "userID": "5",
        ///             "lastName": "Lulu",
        ///             "firstName": "Lala"
        ///         },
        ///         "usersSummary": [
        ///             {
        ///                 "firstName": "test FirstName 1",
        ///                 "lastName": "test LastName 1",
        ///                 "userID": "1",
        ///                 "location": {
        ///                     "locationID": 1,
        ///                     "province": "test Province User 1",
        ///                     "city": "test City User 1"
        ///                 },
        ///                 "utilization": 100,
        ///                 "resourceDiscipline": {
        ///                     "disciplineID": 1,
        ///                     "skills": ["1", "2"],
        ///                     "discipline": "test d1",
        ///                     "yearsOfExp": "3-5"
        ///                 },
        ///                 "isConfirmed": true
        ///             }
        ///         ],
        ///         "openings": [
        ///             {
        ///                 "discipline": "Aerospace engineering?",
        ///                 "positionID": 1111, 
        ///                 "skills": ["Aerospace engineering organizations?"],
        ///                 "yearsOfExp": "1-3",
        ///                 "commitmentMonthlyHours": {
        ///                    "2021-01-01": 30,
        ///                    "2021-02-01": 50
        ///                 }
        ///             },
        ///             {
        ///                 "discipline": "Architecture?",
        ///                 "positionID": 113, 
        ///                 "skills": ["Architectural design?", "Architectural styles?"],
        ///                 "yearsOfExp": "3-5",
        ///                 "commitmentMonthlyHours": {
        ///                     "2021-01-01": 30,
        ///                     "2021-02-01": 50
        ///                 }
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
                var error = new BadRequestException("The given project is null / Request Body cannot be read");
                return StatusCode(StatusCodes.Status400BadRequest, new CustomException<BadRequestException>(error).GetException());
            }

            if (
                projectProfile.ProjectManager == null || String.IsNullOrEmpty(projectProfile.ProjectManager.UserID) ||
                projectProfile.ProjectSummary == null ||
                String.IsNullOrEmpty(projectProfile.ProjectSummary.ProjectNumber) ||
                projectProfile.ProjectSummary.Location == null
            )
            {
                var error = new BadRequestException("The Project (Manager(ID) / Summary / Number / Location) cannot be null or empty string!");
                return StatusCode(StatusCodes.Status400BadRequest, new CustomException<BadRequestException>(error).GetException());
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
                    return StatusCode(StatusCodes.Status500InternalServerError, new CustomException<InternalServerException>(error).GetException());
                }
                else
                {
                    var error = new BadRequestException(errMessage);
                    return StatusCode(StatusCodes.Status400BadRequest, new CustomException<BadRequestException>(error).GetException());
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
        ///                 "locationID": 14,
        ///                 "province": "Alberta",
        ///                 "city": "Lethbridge"
        ///             },
        ///             "projectStartDate": "2020-10-31T00:00:00.0000000",
        ///             "projectEndDate": "2021-02-12T00:00:00.0000000",
        ///             "projectNumber": "0000-0000-05"
        ///         },
        ///         "projectManager": {
        ///             "userID": "5",
        ///             "lastName": "Lulu",
        ///             "firstName": "Lala"
        ///         },
        ///         "usersSummary": [
        ///             {
        ///                 "firstName": "test FirstName 1",
        ///                 "lastName": "test LastName 1",
        ///                 "userID": "1",
        ///                 "location": {
        ///                     "locationID": 1,
        ///                     "province": "test Province User 1",
        ///                     "city": "test City User 1"
        ///                 },
        ///                 "utilization": 100,
        ///                 "resourceDiscipline": {
        ///                     "disciplineID": 1,
        ///                     "skills": ["1", "2"],
        ///                     "discipline": "test d1",
        ///                     "yearsOfExp": "3-5"
        ///                 },
        ///                 "isConfirmed": true
        ///             }
        ///         ],
        ///         "openings": [
        ///             {
        ///                 "discipline": "Aerospace engineering?",
        ///                 "positionID": 1111, 
        ///                 "skills": ["Aerospace engineering organizations?"],
        ///                 "yearsOfExp": "1-3",
        ///                 "commitmentMonthlyHours": {
        ///                    "2021-01-01": 30,
        ///                    "2021-02-01": 50
        ///                 }
        ///             },
        ///             {
        ///                 "discipline": "Architecture?",
        ///                 "positionID": 113, 
        ///                 "skills": ["Architectural design?", "Architectural styles?"],
        ///                 "yearsOfExp": "3-5",
        ///                 "commitmentMonthlyHours": {
        ///                     "2021-01-01": 30,
        ///                     "2021-02-01": 50
        ///                 }
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
                var error = new BadRequestException("The given project profile is null / Request Body cannot be read");
                return StatusCode(StatusCodes.Status400BadRequest, new CustomException<BadRequestException>(error).GetException());
            }

            if (
                projectProfile.ProjectManager == null || String.IsNullOrEmpty(projectProfile.ProjectManager.UserID) ||
                projectProfile.ProjectSummary == null ||
                String.IsNullOrEmpty(projectProfile.ProjectSummary.ProjectNumber) ||
                projectProfile.ProjectSummary.Location == null
            )
            {
                var error = new BadRequestException("The Project (Manager(ID) / Summary / Number / Location) cannot be null or empty string!");
                return StatusCode(StatusCodes.Status400BadRequest, new CustomException<BadRequestException>(error).GetException());
            }

            if (projectProfile.ProjectSummary.ProjectNumber != projectNumber)
            {
                var errMessage = $"The project number on URL '{projectNumber}'" +
                    $" does not match with '{projectProfile.ProjectSummary.ProjectNumber}' in Request Body's Project Summary";
                var error = new BadRequestException(errMessage);
                return StatusCode(StatusCodes.Status400BadRequest, new CustomException<BadRequestException>(error).GetException());
            }

            try
            {
                // Log.Logger.Here().Information("{@Project}", projectProfile);
                var location = await locationsRepository.GetALocation(projectProfile.ProjectSummary.Location.City);
                var updated = await projectsRepository.UpdateAProject(projectProfile, location.Id);
                if (updated == null)
                {
                    var errMessage = $"Query returns failure status on updating project number '{projectProfile.ProjectSummary.ProjectNumber}'";
                    var error = new InternalServerException(errMessage);
                    return StatusCode(StatusCodes.Status500InternalServerError, new CustomException<InternalServerException>(error).GetException());
                }
                var response = new UpdatedResponse<string>(updated, "Successfully updated");
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
                var error = new BadRequestException("The given project number is null");
                return StatusCode(StatusCodes.Status400BadRequest, new CustomException<BadRequestException>(error).GetException());
            }

            try
            {
                var deleted = await projectsRepository.DeleteAProject(projectNumber);
                if (deleted == null)
                {
                    var error = new NotFoundException("The given project number cannot be found on database");
                    return StatusCode(StatusCodes.Status404NotFound, new CustomException<NotFoundException>(error).GetException());
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
                    return StatusCode(StatusCodes.Status500InternalServerError, new CustomException<InternalServerException>(error).GetException());
                }
                else
                {
                    var error = new BadRequestException(errMessage);
                    return StatusCode(StatusCodes.Status400BadRequest,  new CustomException<BadRequestException>(error).GetException());
                }
            }
        }

    /// <summary>Testing GetAPosition</summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET api/projects/"EJ7945NDVCZPWX9"/position/2001
        ///
        /// </remarks>
        /// <param name= "projectNum">The project number as string</param>
        /// <param name= "positionId">The project's id</param>
        /// <returns>The old deleted project</returns>
        /// <response code="201">Returns a RequestProjectAssign (e.g. {{positionId} {userId}})</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized Request</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("projects/{projectNum}/position/{positionId}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedException), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAPosition([FromRoute] string projectNum, int positionId)
        {
            try
            {   
                Position position = await positionsRepository.GetAPosition(positionId);
                
                if (position == null)
                {
                    var error = new NotFoundException("The given positionId cannot be found in the database");
                    return StatusCode(StatusCodes.Status404NotFound, new CustomException<NotFoundException>(error).GetException());
                }

                var response = new UpdatedResponse<Position>(position, "Successfully retrieved");
                return StatusCode(StatusCodes.Status201Created, response);
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
