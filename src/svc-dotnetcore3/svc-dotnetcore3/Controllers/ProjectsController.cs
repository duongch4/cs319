﻿using AutoMapper;
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
        [ProducesResponseType(typeof(OkResponse<IEnumerable<ProjectSummary>>), StatusCodes.Status200OK)]
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
                var resource = mapper.Map<IEnumerable<ProjectResource>, IEnumerable<ProjectSummary>>(projects);
                var response = new OkResponse<IEnumerable<ProjectSummary>>(resource, "Everything is good");
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

                var projectManager = mapper.Map<ProjectResource, ProjectManagerResource>(project);

                var users = await usersRepository.GetAllUsersResourceOnProject(project.Id, project.ManagerId);
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
        /// {
        ///     "projectSummary": {
        ///         "title": "test Title",
        ///         "location": {
        ///             "province": "test Province",
        ///             "city": "Vancouver"
        ///         },
        ///         "projectStartDate": "2020-10-31T00:00:00.0000000",
        ///         "projectEndDate": "2021-02-12T00:00:00.0000000",
        ///         "projectNumber": "0000"
        ///     },
        ///     "projectManager": {
        ///         "userID": 5,
        ///         "lastName": "Lulu",
        ///         "firstName": "Lala"
        ///     },
        ///     "usersSummary": [
        ///         {
        ///             "firstName": "test FirstName 1",
        ///             "lastName": "test LastName 1",
        ///             "userID": 1,
        ///             "location": {
        ///                 "province": "test Province User 1",
        ///                 "city": "test City User 1"
        ///             },
        ///             "utilization": 100,
        ///             "resourceDiscipline": {
        ///                 "discipline": "test d1",
        ///                 "yearsOfExp": "3-5"
        ///             },
        ///             "isConfirmed": true
        ///         },
        ///         {
        ///             "firstName": "test FirstName 2",
        ///             "lastName": "test LastName 2",
        ///             "userID": 2,
        ///             "location": {
        ///                 "province": "test Province User 2",
        ///                 "city": "test City User 2"
        ///             },
        ///             "utilization": 90,
        ///             "resourceDiscipline": {
        ///                 "discipline": "test d1",
        ///                 "yearsOfExp": "3-5"
        ///             },
        ///             "isConfirmed": true
        ///         }
        ///     ],
        ///     "openings": [
        ///         {
        ///             "discipline": "Cryptography",
        ///             "skills": ["Glock", "Kali"],
        ///             "yearsOfExp": "1-3",
        ///             "commitmentMonthlyHours": 160
        ///         },
        ///         {
        ///             "discipline": "Language",
        ///             "skills": ["Mandarin", "False Identity Creation"],
        ///             "yearsOfExp": "3-5",
        ///             "commitmentMonthlyHours": 180
        ///         }
        ///     ]
        /// }
        ///
        /// </remarks>
        /// <param name="projectProfile"></param>
        /// <returns>A newly created project</returns>
        /// <response code="201">Returns the newly created project</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPost]
        [Route("projects")]
        [ProducesResponseType(typeof(CreatedResponse<string>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
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
        ///     PUT /api/projects/2005-KJS4-46
        /// {
        ///     "projectSummary": {
        ///         "title": "test Title",
        ///         "location": {
        ///             "province": "test Province",
        ///             "city": "Vancouver"
        ///         },
        ///         "projectStartDate": "2020-10-31T00:00:00.0000000",
        ///         "projectEndDate": "2021-02-12T00:00:00.0000000",
        ///         "projectNumber": "2005-KJS4-46"
        ///     },
        ///     "projectManager": {
        ///         "userID": 5,
        ///         "lastName": "Lulu",
        ///         "firstName": "Lala"
        ///     },
        ///     "usersSummary": [
        ///         {
        ///             "firstName": "test FirstName 1",
        ///             "lastName": "test LastName 1",
        ///             "userID": 1,
        ///             "location": {
        ///                 "province": "test Province User 1",
        ///                 "city": "test City User 1"
        ///             },
        ///             "utilization": 100,
        ///             "resourceDiscipline": {
        ///                 "discipline": "test d1",
        ///                 "yearsOfExp": "3-5"
        ///             },
        ///             "isConfirmed": true
        ///         },
        ///         {
        ///             "firstName": "test FirstName 2",
        ///             "lastName": "test LastName 2",
        ///             "userID": 2,
        ///             "location": {
        ///                 "province": "test Province User 2",
        ///                 "city": "test City User 2"
        ///             },
        ///             "utilization": 90,
        ///             "resourceDiscipline": {
        ///                 "discipline": "test d1",
        ///                 "yearsOfExp": "3-5"
        ///             },
        ///             "isConfirmed": true
        ///         }
        ///     ],
        ///     "openings": [
        ///         {
        ///             "discipline": "Cryptography",
        ///             "skills": ["Glock", "Kali"],
        ///             "yearsOfExp": "1-3",
        ///             "commitmentMonthlyHours": 160
        ///         },
        ///         {
        ///             "discipline": "Language",
        ///             "skills": ["Mandarin", "False Identity Creation"],
        ///             "yearsOfExp": "3-5",
        ///             "commitmentMonthlyHours": 180
        ///         }
        ///     ]
        /// }
        ///
        /// </remarks>
        /// <param name="projectProfile"></param>
        /// <param name="projectNumber"></param>
        /// <returns>An updated project</returns>
        /// <response code="200">Returns the updated project</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPut]
        [Route("projects/{projectNumber}")]
        [ProducesResponseType(typeof(UpdatedResponse<ProjectProfile>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
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
        /// <returns>The old deleted project number</returns>
        /// <response code="200">Returns the old deleted project number</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">If no projects are found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpDelete]
        [Route("projects/{projectNumber}")]
        [ProducesResponseType(typeof(DeletedResponse<string>), StatusCodes.Status200OK)]
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
