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
        [ProducesResponseType(typeof(OkResponse<IEnumerable<UserSummary>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await usersRepository.GetAllUsersGeneral();
                if (users == null || !users.Any())
                {
                    return StatusCode(StatusCodes.Status404NotFound, new NotFoundException("No users data found"));
                }
                var resource = mapper.Map<IEnumerable<UserResource>, IEnumerable<UserSummary>>(users);
                var response = new OkResponse<IEnumerable<UserSummary>>(resource, "Everything is good");
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

                var location = await locationsRepository.GetUserLocation(user);
                var userResource = new UserResource{
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Username = user.Username,
                    LocationId = user.LocationId,
                    Province = location.Province,
                    City = location.City,
                    IsConfirmed = false,
                    DisciplineName = null,
                    YearsOfExperience = null
                 };
                var userSummary = mapper.Map<UserResource, UserSummary>(userResource);

                var projects = await projectsRepository.GetAllProjectsOfUser(user);
                var positions = await positionsRepository.GetPositionsOfUser(user);
                var disciplines = await disciplinesRepository.GetUserDisciplines(user);
                var skills = await skillsRepository.GetUserSkills(user);
                var utilization = positions.Aggregate(0, (result, x) => result + x.ProjectedMonthlyHours);
                var outOfOffice = await outOfOfficeRepository.GetAllOutOfOfficeForUser(user);
                IEnumerable<RDisciplineResource> disciplineResources = Enumerable.Empty<RDisciplineResource>();
                foreach (var discipline in disciplines)
                {
                    var discSkills = skills.Where(x => x.ResourceDisciplineName == discipline.Name);
                    var disc = new RDisciplineResource{
                        Discipline = discipline.Name,
                        YearsOfExperience = discipline.YearsOfExperience,
                        Skills = discSkills.Select(x => x.Name).ToList()
                    };
                    disciplineResources = disciplineResources.Append(disc);
                }
                var userProfile = new UserProfileResource{
                    UserSummary = userSummary,
                    Availability = mapper.Map<IEnumerable<OutOfOffice>, IEnumerable<OutOfOfficeResource>>(outOfOffice),
                    CurrentProjects = mapper.Map<IEnumerable<Project>, IEnumerable<ProjectDirectMappingResource>>(projects),
                    Disciplines = disciplineResources,
                    Positions = positions
                };

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
        ///     PUT /api/users/
        ///     {
        ///         "userSummary": {
        ///             "userId": 3,
        ///             "firstName": "Natasha",
        ///             "lastName": "Romanov",
        ///             "location": {
        ///                 "province": "British Columbia",
        ///                 "city": "Vancouver"
        ///             },
        ///             "utilization": 170
        ///         },
        ///         "currentProjects": [
        ///             {
        ///                 "id": 2,
        ///                 "title": "Budapest",
        ///                 "locationId": 19,
        ///                 "projectStartDate": "2020-04-19T00:00:00",
        ///                 "projectEndDate": "2020-07-01T00:00:00"
        ///             }
        ///         ],
        ///         "availability": [
        ///             {
        ///                 "fromDate": "2020-10-31T00:00:00",
        ///                 "toDate": "2020-11-11T00:00:00",
        ///                 "reason": "Maternal Leave"
        ///             }
        ///         ],
        ///         "disciplines": [
        ///             {
        ///                 "name": "Weapons",
        ///                 "yearsOfExperience": "10+",
        ///                 "skills": [
        ///                     "Glock", "Sniper Rifle"
        ///                 ]
        ///             }
        ///         ]
        ///     }
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
                UserSummary summary = user.UserSummary;
                Location location = await locationsRepository.GetLocationIdByCityProvince(summary.Location);
                User updateUser = createUserFromSummary(summary, location);
                var changedUser = await usersRepository.UpdateAUser(updateUser);
                var disciplines = await processDisciplineSkillChanges(user.Disciplines, updateUser);
                var avails = await processOutOfOfficeChanges(user.Availability, updateUser);
                var tmp = new { changedUser, disciplines, avails };
                var response = new OkResponse<int>(updateUser.Id, "Successfully updated");
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
        private User createUserFromSummary(UserSummary summary, Location location)
        {
            var user = new User {
                Id = summary.UserID,
                FirstName = summary.FirstName,
                LocationId = location.Id,
                LastName = summary.LastName
            };
            return user;
        }

        private IEnumerable<ResourceDiscipline> createResourceDisciplinesFromProfile(IEnumerable<RDisciplineResource> disciplines, int userId)
        {
            var result = Enumerable.Empty<ResourceDiscipline>();
            foreach (var discipline in disciplines)
            {
                var disc = new ResourceDiscipline{
                    ResourceId = userId,
                    Name = discipline.Discipline,
                    YearsOfExperience = discipline.YearsOfExperience
                };
                result = result.Append(disc);
            }
            return result;
        }

        private async Task<Object> processDisciplineSkillChanges(IEnumerable<RDisciplineResource> profile, User user)
        {
            var profileDisciplines = createResourceDisciplinesFromProfile(profile, user.Id);
            var profileSkills = createResourceSkillsFromProfile(profile, user.Id);
            // Log.Logger.Information("finish getting profile Skills" + profileSkills);
            var disciplinesDB = await disciplinesRepository.GetUserDisciplines(user);
            // Log.Logger.Information("finish getting Discipline DB");
            var skillsDB = await skillsRepository.GetUserSkills(user);
            // Log.Logger.Information("finish getting skill DB");
            bool isSameDisc = disciplinesDB.SequenceEqual(profileDisciplines);
            bool isSameSkill = skillsDB.SequenceEqual(profileSkills);
            if (!isSameDisc || !isSameSkill)
            {
                var removedSkill = await removeSkillsFromDB(profileSkills, skillsDB);
                var removed = await removeDisciplinesFromDB(profileDisciplines, disciplinesDB);
                var inserted = await addDisciplinesToDB(profileDisciplines, disciplinesDB);
                var insertedSkill = await addSkillsToDB(profileSkills, skillsDB);
                return new {removed, removedSkill, insertedSkill, inserted};
            }
            return null;
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
                    sk.ResourceDisciplineName = disc.Discipline;
                    sk.Name = skill;
                    result = result.Append(sk);
                }
            }
            // Log.Logger.Information("complete loop in skill creation");
            return result;
        }

        private async Task<IEnumerable<ResourceSkill>> addSkillsToDB(IEnumerable<ResourceSkill> profile, IEnumerable<ResourceSkill> db)
        {
            var toBeAdded = profile.Except(db);
            var result = Enumerable.Empty<ResourceSkill>();
            foreach (var skill in toBeAdded)
            {
                var added = await skillsRepository.InsertResourceSkill(skill);
                result = result.Append(added);
            }
            return result;
        }

        private async Task<IEnumerable<ResourceSkill>> removeSkillsFromDB(IEnumerable<ResourceSkill> profile, IEnumerable<ResourceSkill> db)
        {
            var toBeRemoved = db.Except(profile);
            var result = Enumerable.Empty<ResourceSkill>();
            foreach (var skill in toBeRemoved)
            {
                var removed = await skillsRepository.DeleteResourceSkill(skill);
                result = result.Append(removed);
            }
            return result;
        }
        private async Task<IEnumerable<ResourceDiscipline>> addDisciplinesToDB(IEnumerable<ResourceDiscipline> profile, IEnumerable<ResourceDiscipline> db)
        {
            var toBeAdded = profile.Except(db);
            var result = Enumerable.Empty<ResourceDiscipline>();
            foreach (var disc in toBeAdded)
            {
                var added = await disciplinesRepository.InsertResourceDiscipline(disc);
                result = result.Append(added);
            }
            return result;
        }

        private async Task<IEnumerable<ResourceDiscipline>> removeDisciplinesFromDB(IEnumerable<ResourceDiscipline> profile, IEnumerable<ResourceDiscipline> db)
        {
            var toBeRemoved = db.Except(profile);
            var result = Enumerable.Empty<ResourceDiscipline>();
            foreach (var disc in toBeRemoved)
            {
                var removed = await disciplinesRepository.DeleteResourceDiscipline(disc);
                result = result.Append(removed);
            }
            return result;
        }

        private IEnumerable<OutOfOffice> createOutOfOfficeFromProfile(IEnumerable<OutOfOfficeResource> availabilities, int userId)
        {
            var result = Enumerable.Empty<OutOfOffice>();
            foreach (var availability in availabilities)
            {
                var avail = new OutOfOffice{
                    ResourceId = userId,
                    FromDate = availability.FromDate,
                    ToDate = availability.ToDate,
                    Reason = availability.Reason
                };
                result = result.Append(avail);
            }
            return result;
        }

        private async Task<IEnumerable<OutOfOffice>> processOutOfOfficeChanges(IEnumerable<OutOfOfficeResource> profile, User user)
        {
            var profileAvailability = createOutOfOfficeFromProfile(profile, user.Id);
            var availabilityDB = await outOfOfficeRepository.GetAllOutOfOfficeForUser(user);
            var result = Enumerable.Empty<OutOfOffice>();
            bool isSameAvail = profileAvailability.SequenceEqual(availabilityDB);
            if (!isSameAvail)
            {
                Log.Logger.Information("avail function " + isSameAvail);
                var removed = await removeAvailFromDB(profileAvailability, availabilityDB);
                var inserted = await addAvailToDB(profileAvailability, availabilityDB);
                result = removed.Concat(inserted);
            }
            return result;
        }

        private async Task<IEnumerable<OutOfOffice>> removeAvailFromDB(IEnumerable<OutOfOffice> profile, IEnumerable<OutOfOffice> db)
        {
            var toBeRemoved = db.Except(profile);
            var result = Enumerable.Empty<OutOfOffice>();
            foreach (var avail in toBeRemoved)
            {
                var removed = await outOfOfficeRepository.DeleteOutOfOffice(avail);
                result = result.Append(removed);
            }
            return result;
        }

        private async Task<IEnumerable<OutOfOffice>> addAvailToDB(IEnumerable<OutOfOffice> profile, IEnumerable<OutOfOffice> db)
        {
            var toBeAdded = profile.Except(db);
            var result = Enumerable.Empty<OutOfOffice>();
            foreach (var avail in toBeAdded)
            {
                var added = await outOfOfficeRepository.InsertOutOfOffice(avail);
                result = result.Append(added);
            }
            return result;
        }
    }

    // [Authorize]
    // public class OldUsersController : ControllerBase
    // {
    //     private readonly IUsersRepository usersRepository;
    //     private readonly IMapper mapper;

    //     public OldUsersController(IUsersRepository usersRepository, IMapper mapper)
    //     {
    //         this.usersRepository = usersRepository;
    //         this.mapper = mapper;
    //     }

    //     [HttpGet]
    //     [Route("/users")]
    //     public async Task<ActionResult<IEnumerable<User>>> GetAllUsersOld()
    //     {
    //         var response = await usersRepository.GetAllUsers();
    //         var viewModel = mapper.Map<IEnumerable<User>>(response);
    //         return Ok(viewModel);
    //     }
    // }
}
