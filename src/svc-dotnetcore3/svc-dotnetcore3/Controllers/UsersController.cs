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
                var positions = await positionsRepository.GetPositionsOfUser(user);
                var disciplines = await disciplinesRepository.GetUserDisciplines(user);
                var skills = await skillsRepository.GetUserSkills(user);
                var utilization = positions.Aggregate(0, (result, x) => result + x.ProjectedMonthlyHours);
                var location = await locationsRepository.GetUserLocation(user);
                var outOfOffice = await outOfOfficeRepository.GetAllOutOfOfficeForUser(user);
                IEnumerable<RDisciplineResource> disciplineResources = Enumerable.Empty<RDisciplineResource>();
                foreach (var discipline in disciplines)
                {
                    var disc = new RDisciplineResource();
                    disc.Name = discipline.DisciplineName;
                    disc.YearsOfExperience = discipline.YearsOfExperience;
                    var discSkills = skills.Where(x => x.ResourceDisciplineName == discipline.DisciplineName);
                    disc.Skills = discSkills.Select(x => x.Name).ToList();
                    disciplineResources = disciplineResources.Append(disc);
                }
                var userSummary = new UserSummaryResource();
                userSummary.firstName = user.FirstName;
                userSummary.lastName = user.LastName;
                userSummary.Discipline = null;
                userSummary.Position = null;
                userSummary.Utilization = utilization;
                userSummary.Location = mapper.Map<Location, LocationResource>(location);
                userSummary.userID = user.Id;
                userSummary.isConfirmed = false;
                var userProfile = new UserProfileResource();
                userProfile.UserSummary = userSummary;
                userProfile.Availability = mapper.Map<IEnumerable<OutOfOffice>, IEnumerable<OutOfOfficeResource>>(outOfOffice);
                userProfile.CurrentProjects = mapper.Map<IEnumerable<Project>, IEnumerable<ProjectDirectMappingResource>>(projects);
                userProfile.Disciplines = disciplineResources;

                var response = new OkResponse<UserProfileResource>(userProfile, "Everything is good");
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

        /// <summary>Update user</summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/users/5
        ///
        /// </remarks>
        /// <returns>The id of the user that was updated</returns>
        /// <response code="200">Returns the userId</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">If the requested user cannot be found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPut]
        [Route("users/")]
        [ProducesResponseType(typeof(OkResponse<UserResource>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUser([FromBody] UserProfileResource user)
        {
            if (user == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new BadRequestException("The given user is null / Request Body cannot be read"));
            }

            try
            {
                UserSummaryResource summary = user.UserSummary;
                User updateUser = createUserFromSummary(summary);
                // // IEnumerable<ResourceDisciplines> profileDisciplines = createResourceDisciplinesFromProfile(user.Disciplines, summary.UserId);
                // // IEnumerable<ResourceSkill> profileSkills = createResourceSkillsFromProfile(user.Disciplines, summary.UserId);
                //IEnumerable<OutOfOffice> profileAvailability = createOutOfOfficeFromProfile(user.Availability, summary.userID);
                // // var disciplinesInDB = await disciplinesRepository.GetUserDisciplines(updateUser);
                // // var skillsInDB = await skillsRepository.GetUserSkills(updateUser);
                // var availabilitiesInDB = await outOfOfficeRepository.GetAllOutOfOfficeForUser(updateUser);
                var avails = await processOutOfOfficeChanges(user.Availability, updateUser);

                // // var areSame = disciplinesInDB.SequenceEqual(profileDisciplines);
                // // if (!areSame)
                // // {
                // //     addMissingDisciplinesToDB(disciplinesInDB, profileDisciplines);
                // //     removeDisciplinesFromDB(disciplinesInDB, profileDisciplines);
                // // }
                // // var addToDB = profileDisciplines.Except(disciplinesInDB);
                // // var deleteFromDB = disciplinesInDB.Except(profileDisciplines);

                // // var sameSkills = skillsInDB.SequenceEqual(profileSkills);
                // // var resource = await usersRepository.UpdateAUser(updateUser);
                // var tmp = new { updateUser, hasSameAvail, availabilitiesInDB, profileAvailability };
                // var response = tmp;/* new OkResponse<int>(summary.UserId, "Successfully updated"); */
                return StatusCode(StatusCodes.Status200OK, avails);
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
        private User createUserFromSummary(UserSummaryResource summary)
        {
            var user = new User();
            user.LocationId = summary.Location.Id;
            user.Id = summary.userID;
            user.FirstName =summary.firstName;
            user.LastName = summary.lastName;
            return user;
        }

        private IEnumerable<ResourceDisciplines> createResourceDisciplinesFromProfile(IEnumerable<RDisciplineResource> disciplines, int userId)
        {
            var result = Enumerable.Empty<ResourceDisciplines>();
            foreach (var discipline in disciplines)
            {
                var disc = new ResourceDisciplines();
                disc.ResourceId = userId;
                disc.DisciplineName = discipline.Name;
                disc.YearsOfExperience = discipline.YearsOfExperience;
                result = result.Append(disc);
            }
            return result;
        }

        private IEnumerable<ResourceSkill> createResourceSkillsFromProfile(IEnumerable<RDisciplineResource> disciplines, int userId)
        {
            var result = Enumerable.Empty<ResourceSkill>();
            foreach (var disc in disciplines)
            {
                foreach (var skill in disc.Skills)
                {
                    var sk = new ResourceSkill();
                    sk.ResourceId = userId;
                    sk.ResourceDisciplineName = disc.Name;
                    sk.Name = skill;
                    result = result.Append(sk);
                }
            }
            return result;
        }

        private void addMissingDisciplinesToDB(IEnumerable<ResourceDisciplines> db, IEnumerable<ResourceDisciplines> profile)
        {
            var addToDB = profile.Except(db);
            Log.Logger.Information("addMissingDisciplines" + addToDB);
        }

        private void removeDisciplinesFromDB(IEnumerable<ResourceDisciplines> db, IEnumerable<ResourceDisciplines> profile)
        {
            var removeFromDB = db.Except(profile);
            Log.Logger.Information("removeDisciplines");
        }

        private IEnumerable<OutOfOffice> createOutOfOfficeFromProfile(IEnumerable<OutOfOfficeResource> availabilities, int userId)
        {
            var result = Enumerable.Empty<OutOfOffice>();
            foreach (var availability in availabilities)
            {
                var avail = new OutOfOffice();
                avail.ResourceId = userId;
                avail.FromDate = availability.FromDate;
                avail.ToDate = availability.ToDate;
                avail.Reason = availability.Reason;
                result = result.Append(avail);
            }
            return result;
        }
    
        private async Task<IEnumerable<OutOfOffice>> processOutOfOfficeChanges(IEnumerable<OutOfOfficeResource> profile, User user) {
            var profileAvailability = createOutOfOfficeFromProfile(profile, user.Id);
            var availabilityDB = await outOfOfficeRepository.GetAllOutOfOfficeForUser(user);
            var result = Enumerable.Empty<OutOfOffice>();
            bool isSameAvail = profileAvailability.SequenceEqual(availabilityDB);
            if(!isSameAvail) {
                Log.Logger.Information("avail function " + isSameAvail);
                // var removed = await removeAvailFromDB(profileAvailability, availabilityDB);
                /* var inserted =  */ return await addAvailToDB(profileAvailability, availabilityDB);
                // result = removed.Concat(inserted);
            }
            return result;
        }

        private async Task<IEnumerable<OutOfOffice>> removeAvailFromDB(IEnumerable<OutOfOffice> profile, IEnumerable<OutOfOffice> db) {
            var toBeRemoved = db.Except(profile);
            var result = Enumerable.Empty<OutOfOffice>();
            foreach(var avail in toBeRemoved) {
                var removed = await outOfOfficeRepository.DeleteOutOfOffice(avail);
                result = result.Append(removed);
            }
            return result;
        }

        private async Task<IEnumerable<OutOfOffice>> addAvailToDB(IEnumerable<OutOfOffice> profile, IEnumerable<OutOfOffice> db) {
            var toBeRemoved = profile.Except(db);
            var result = Enumerable.Empty<OutOfOffice>();
            foreach(var avail in toBeRemoved) {
                var removed = await outOfOfficeRepository.InsertOutOfOffice(avail);
                result = result.Append(removed);
            }
            return result;
        }
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
