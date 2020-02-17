using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public class UsersController : ControllerBase
    {
        private readonly IUsersRepository usersRepository;
        private readonly IProjectsRepository projectsRepository;
        private readonly IPositionsRepository positionsRepository;
        private readonly ILocationsRepository locationsRepository;
        private readonly IDisciplinesRepository disciplinesRepository;
        private readonly ISkillsRepository skillsRepository;
        private readonly IOutOfOfficeRepository outOfOfficeRepository;
        private readonly IMapper mapper;

        public UsersController(IUsersRepository usersRepository, IProjectsRepository projectsRepository, IPositionsRepository positionsRepository, 
            ILocationsRepository locationsRepository, IDisciplinesRepository disciplinesRepository, ISkillsRepository skillsRepository, IOutOfOfficeRepository outOfOfficeRepository, IMapper mapper)
        {
            this.usersRepository = usersRepository;
            this.projectsRepository = projectsRepository;
            this.positionsRepository = positionsRepository;
            this.locationsRepository = locationsRepository;
            this.disciplinesRepository = disciplinesRepository;
            this.skillsRepository = skillsRepository;
            this.outOfOfficeRepository = outOfOfficeRepository;
            this.mapper = mapper;
        }

        /// <summary>Get all users</summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/users
        ///
        /// </remarks>
        /// <returns>All available users</returns>
        /// <response code="200">Returns all available users</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">If no users are found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("users")]
        [ProducesResponseType(typeof(OkResponse<IEnumerable<UserResource>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await usersRepository.GetAllUsers();
                if (users == null || !users.Any())
                {
                    return StatusCode(StatusCodes.Status404NotFound, new NotFoundException("No users data found"));
                }
                var resource = mapper.Map<IEnumerable<User>, IEnumerable<UserResource>>(users);
                var response = new OkResponse<IEnumerable<UserResource>>(resource, "Everything is good");
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

        /// <summary>Get a specific user based on a given userId</summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/users/5
        ///
        /// </remarks>
        /// <param name="userId"></param>
        /// <returns>The requested user</returns>
        /// <response code="200">Returns the requested user</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">If the requested user cannot be found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("users/{userId}")]
        [ProducesResponseType(typeof(OkResponse<UserResource>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAUser(string userId)
        {
            if (userId == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new BadRequestException("The given userId is null"));
            }
            try
            {
                var userIdInt = Int32.Parse(userId);
                var user = await usersRepository.GetAUser(userIdInt);
                if (user == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new NotFoundException($"No users with userId '{userId}' found"));
                }

                // var userResource = mapper.Map<User, UserResource>(user);

                var projects = await projectsRepository.GetAllProjectsOfUser(user);
                 var projectResources = mapper.Map<IEnumerable<Project>, IEnumerable<ProjectDirectMappingResource>>(projects);
                var positions = await positionsRepository.GetPositionsOfUser(user);
                var disciplines = await disciplinesRepository.GetUserDisciplines(user);
                var skills = await skillsRepository.GetUserSkills(user);
                var skillNames = skills.Select(x => x.Name);
                var utilization = positions.Aggregate(0, (result, x) => result + x.ProjectedMonthlyHours);
                var location = await locationsRepository.GetUserLocation(user);
                var outOfOffice = await outOfOfficeRepository.GetAllOutOfOfficeForUser(user);
                var availability = mapper.Map<IEnumerable<OutOfOffice>, IEnumerable<OutOfOfficeResource>>(outOfOffice);
                var userSummary = new {
                    name = user.FirstName + " " + user.LastName,
                    discipline = "none",
                    position = "none",
                    utilization,
                    location = location,
                    userID = user.Id
                };
                var userProfile = new {
                    UserSummary = userSummary,
                    currentProjects = projectResources,
                    availability,
                    disciplines,
                    skills
                };
                // var tmpuserSummary = new UserSummaryResource(user, null, null, positions, location, mapper);
                // var tmpuserProfile = new UserProfileResource(null, projects, outOfOffice, disciplines, skills, mapper);
                // var tmp = new {tmpuserSummary, tmpuserProfile};
                var response = userProfile; /* new OkResponse<UserProfileResource>(userProfile, "Everything is good"); */
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

        // /// <summary>Update user</summary>
        // /// <remarks>
        // /// Sample request:
        // ///
        // ///     PUT /api/users/5
        // ///
        // /// </remarks>
        // /// <returns>The id of the user that was updated</returns>
        // /// <response code="200">Returns the userId</response>
        // /// <response code="400">Bad Request</response>
        // /// <response code="404">If the requested user cannot be found</response>
        // /// <response code="500">Internal Server Error</response>
        // [HttpPut]
        // [Route("users/{userId}")]
        // [ProducesResponseType(typeof(OkResponse<UserResource>), StatusCodes.Status200OK)]
        // [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        // [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        // [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]
        // public async Task<IActionResult> UpdateUser([FromBody] UserProfile user) {

        // }
    }

    [Authorize]
    public class OldUsersController : ControllerBase
    {
        private readonly IUsersRepository usersRepository;
        private readonly IMapper mapper;

        public OldUsersController(IUsersRepository usersRepository, IMapper mapper)
        {
            this.usersRepository = usersRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        [Route("/users")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsersOld()
        {
            var response = await usersRepository.GetAllUsers();
            var viewModel = mapper.Map<IEnumerable<User>>(response);
            return Ok(viewModel);
        }
    }
}
