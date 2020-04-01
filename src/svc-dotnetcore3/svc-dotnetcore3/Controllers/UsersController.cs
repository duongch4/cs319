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
using Web.API.Authorization;

using System;
using System.Data.SqlClient;
using System.Linq;
using Serilog;

namespace Web.API.Controllers
{
    [Authorize(Actions.RegularThings)]
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
        private readonly IUtilizationRepository utilizationRepository;
        private readonly IMapper mapper;

        public UsersController(
            IUsersRepository usersRepository, IProjectsRepository projectsRepository, IPositionsRepository positionsRepository,
            ILocationsRepository locationsRepository, IDisciplinesRepository disciplinesRepository,
            ISkillsRepository skillsRepository, IOutOfOfficeRepository outOfOfficeRepository,
            IUtilizationRepository utilizationRepository, IMapper mapper
        )
        {
            this.usersRepository = usersRepository;
            this.projectsRepository = projectsRepository;
            this.positionsRepository = positionsRepository;
            this.locationsRepository = locationsRepository;
            this.disciplinesRepository = disciplinesRepository;
            this.skillsRepository = skillsRepository;
            this.outOfOfficeRepository = outOfOfficeRepository;
            this.utilizationRepository = utilizationRepository;
            this.mapper = mapper;
        }

        /// <summary>Get all users</summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/users?orderKey={orderKey}&#38;order={order}&#38;page={pageNumber}
        ///
        /// </remarks>
        /// <param name="searchWord" />
        /// <param name="orderKey" />
        /// <param name="order" />
        /// <param name="page" />
        /// <returns>All available users</returns>
        /// <response code="200">Returns all available users</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized Request</response>
        /// <response code="404">If no users are found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("users")]
        [ProducesResponseType(typeof(OkResponse<IEnumerable<UserSummary>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedException), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllUsers([FromQuery] string searchWord, [FromQuery] string orderKey, [FromQuery] string order, [FromQuery] int page)
        {
            orderKey = (orderKey == null) ? "utilization" : orderKey;
            order = (order == null) ? "desc" : order;
            page = (page == 0) ? 1 : page;
            try
            {
                var users = await usersRepository.GetAllUserResources(searchWord, orderKey, order, page);
                if (users == null || !users.Any())
                {
                    var error = new NotFoundException("No users data found");
                    return StatusCode(StatusCodes.Status404NotFound, new CustomException<NotFoundException>(error).GetException());
                }
                var resource = mapper.Map<IEnumerable<UserResource>, IEnumerable<UserSummary>>(users);
                var extra = new {
                    searchWord = searchWord,
                    page = page,
                    size = resource.Count(),
                    order = order,
                    orderKey = orderKey
                };
                var response = new OkResponse<IEnumerable<UserSummary>>(resource, "Everything is good", extra);
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

        /// <summary>Get a specific user based on a given userId</summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/users/2
        ///
        /// </remarks>
        /// <param name="userId"></param>
        /// <returns>The requested user</returns>
        /// <response code="200">Returns the requested user</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized Request</response>
        /// <response code="404">If the requested user cannot be found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("users/{userId}")]
        [ProducesResponseType(typeof(OkResponse<UserProfile>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedException), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAUser(string userId)
        {
            try
            {
                var user = await usersRepository.GetAUserResource(userId);
                if (user == null)
                {
                    var error = new NotFoundException($"No users with userId '{userId}' found");
                    return StatusCode(StatusCodes.Status404NotFound, new CustomException<NotFoundException>(error).GetException());
                }
                var userSummary = mapper.Map<UserResource, UserSummary>(user);

                var projects = await projectsRepository.GetAllProjectResourcesOfUser(userId);
                var positionsResource = await positionsRepository.GetPositionsOfUser(userId);
                var positions = mapper.Map<IEnumerable<PositionResource>, IEnumerable<PositionSummary>>(positionsResource);
                var disciplines = await disciplinesRepository.GetUserDisciplines(userId);
                var skills = await skillsRepository.GetUserSkills(userId);
                var outOfOffice = await outOfOfficeRepository.GetAllOutOfOfficeForUser(userId);
                IEnumerable<ResourceDisciplineResource> disciplineResources = Enumerable.Empty<ResourceDisciplineResource>();
                foreach (var discipline in disciplines)
                {
                    var discSkills = skills.Where(x => x.ResourceDisciplineName == discipline.Name);
                    var disc = new ResourceDisciplineResource{
                        DisciplineID = discipline.DisciplineId,
                        Discipline = discipline.Name,
                        YearsOfExp = discipline.YearsOfExperience,
                        Skills = discSkills.Select(x => x.Name).ToHashSet()
                    };
                    disciplineResources = disciplineResources.Append(disc);
                }
                var userProfile = new UserProfile{
                    UserSummary = userSummary,
                    Availability = mapper.Map<IEnumerable<OutOfOffice>, IEnumerable<OutOfOfficeResource>>(outOfOffice),
                    CurrentProjects = mapper.Map<IEnumerable<ProjectResource>, IEnumerable<ProjectSummary>>(projects),
                    Disciplines = disciplineResources,
                    Positions = positions
                };

                var response = new OkResponse<UserProfile>(userProfile, "Everything is good");
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

        /// <summary>Update user</summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/users/2
        /// {
        ///     "userSummary": {
        ///       "userID": "2",
        ///       "firstName": "F",
        ///      "lastName": "ChanChan",
        ///       "location": {
        ///         "locationID": 20,
        ///         "province": "Ontario",
        ///         "city": "Kitchener"
        ///       },
        ///       "utilization": 0,
        ///       "resourceDiscipline": {
        ///         "disciplineID": 0,
        ///         "discipline": null,
        ///         "yearsOfExp": null,
        ///         "skills": []
        ///       },
        ///       "isConfirmed": false
        ///     },
        ///     "currentProjects": [
        ///       {
        ///         "title": "Appropriate Venom",
        ///         "location": {
        ///           "locationID": 14,
        ///           "province": "Alberta",
        ///           "city": "Lethbridge"
        ///         },
        ///         "projectStartDate": "2030-12-01T00:00:00",
        ///         "projectEndDate": "2030-12-05T00:00:00",
        ///         "projectNumber": "7ETHF1Y12JFS7XJ"
        ///       },
        ///       {
        ///         "title": "Rebel Canal",
        ///         "location": {
        ///           "locationID": 5,
        ///           "province": "British Columbia",
        ///           "city": "Kamloops"
        ///         },
        ///         "projectStartDate": "2024-02-17T00:00:00",
        ///         "projectEndDate": "2025-08-26T00:00:00",
        ///         "projectNumber": "LN2C78HG8UECD90"
        ///       }
        ///     ],
        ///     "availability": [
        ///       {
        ///         "fromDate": "2020-10-31T00:00:00",
        ///         "toDate": "2020-11-11T00:00:00",
        ///         "reason": "Disneyland Vacation"
        ///       }],
        ///     "disciplines": [
        ///       {
        ///         "disciplineID": 1,
        ///         "discipline": "Aerospace engineering?",
        ///         "yearsOfExp": "10+",
        ///         "skills": [
        ///           "Aircraft components?"
        ///         ]
        ///       }
        ///     ],
        ///     "positions": [
        ///       {
        ///         "positionID": 1400,
        ///         "positionName": "\"\"",
        ///         "projectTitle": "Appropriate Venom",
        ///         "disciplineName": "Offshore engineering?",
        ///         "projectedMonthlyHours": {
        ///           "2021-01-03": 115,
        ///           "2021-02-23": 126,
        ///           "2021-03-25": 24,
        ///           "2021-04-08": 200,
        ///           "2021-05-24": 96,
        ///           "2021-06-10": 35,
        ///           "2021-07-19": 18,
        ///           "2021-08-25": 81,
        ///           "2021-09-02": 166,
        ///           "2021-10-17": 145,
        ///           "2021-11-24": 198,
        ///           "2021-12-02": 152
        ///         }
        ///       },
        ///       {
        ///         "positionID": 1866,
        ///         "positionName": "\"\"",
        ///         "projectTitle": "Rebel Canal",
        ///         "disciplineName": "Smart materials?",
        ///         "projectedMonthlyHours": {
        ///           "2022-01-25": 21,
        ///           "2022-02-24": 7,
        ///           "2022-03-04": 142,
        ///           "2022-04-24": 21,
        ///           "2022-05-02": 30,
        ///           "2022-06-05": 83,
        ///           "2022-07-16": 49,
        ///           "2022-08-21": 91,
        ///           "2022-09-08": 178,
        ///           "2022-10-27": 122,
        ///           "2022-11-05": 34,
        ///           "2022-12-20": 158
        ///         }
        ///       }
        ///     ]
        ///   }
        ///
        /// </remarks>
        /// <param name="userProfile"></param>
        /// <param name="userId"></param>
        /// <returns>The id of the user that was updated</returns>
        /// <response code="200">Returns the userId</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized Request</response>
        /// <response code="404">If the requested user cannot be found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPut]
        [Route("users/{userId}")]
        [ProducesResponseType(typeof(OkResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedException), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUser([FromBody] UserProfile userProfile, string userId)
        {
            if (userProfile == null || userProfile.UserSummary == null || userProfile.UserSummary.Location == null || userProfile.Availability == null || userProfile.Disciplines == null)
            {
                var error = new BadRequestException("The given user is null / Request Body cannot be read");
                return StatusCode(StatusCodes.Status400BadRequest, new CustomException<BadRequestException>(error).GetException());
            }

            if (!String.Equals(userProfile.UserSummary.UserID, userId))
            {
                var errMessage = $"The user ID on URL '{userId}'" +
                    $" does not match with '{userProfile.UserSummary.UserID}' in Request Body's Project Summary";
                var error = new BadRequestException(errMessage);
                return StatusCode(StatusCodes.Status400BadRequest, new CustomException<BadRequestException>(error).GetException());
            }

            try
            {
                UserSummary summary = userProfile.UserSummary;
                Location location = await locationsRepository.GetALocation(userProfile.UserSummary.Location.City);
                var changedUser = await usersRepository.UpdateAUser(summary, location);
                if (changedUser == "-1")
                {
                    var error = new InternalServerException($"Cannot Update User with id: {userId}");
                    return StatusCode(StatusCodes.Status500InternalServerError, new CustomException<InternalServerException>(error).GetException());
                }
                var disciplines = await processDisciplineSkillChanges(userProfile.Disciplines, userId);
                var avails = await processOutOfOfficeChanges(userProfile.Availability, userId);
                var tmp = new { changedUser, disciplines, avails };
                var response = new OkResponse<string>(userId, "Successfully updated");
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

        /// <summary>Update Utilization</summary>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        /// <response code="200"></response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized Request</response>
        /// <response code="500">Internal Server Error</response>
        [Authorize(Actions.AdminThings)]
        [HttpPut]
        [Route("users/utilization/update")]
        [ProducesResponseType(typeof(OkResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedException), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUtilizationOfAllUsers()
        {
            try
            {
                IEnumerable<User> allUsers = await usersRepository.GetAllUsers();
                foreach (User user in allUsers) {
                    IEnumerable<Position> allPositionsOfUser = await positionsRepository.GetAllPositionsOfUser(user.Id);
                    IEnumerable<OutOfOffice> allOutOfOfficeOfUser = await outOfOfficeRepository.GetAllOutOfOfficeForUser(user.Id);
                    int userUtil = await utilizationRepository.CalculateUtilizationOfUser(allPositionsOfUser, allOutOfOfficeOfUser);
                    await usersRepository.UpdateUtilizationOfUser(userUtil, user.Id);
                }

                var response = new OkResponse<string>(null, "Successfully updated all Users");
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

        private IEnumerable<ResourceDiscipline> createResourceDisciplinesFromProfile(IEnumerable<ResourceDisciplineResource> disciplines, string userId)
        {
            var result = Enumerable.Empty<ResourceDiscipline>();
            foreach (var discipline in disciplines)
            {
                var disc = new ResourceDiscipline{
                    ResourceId = userId,
                    Name = discipline.Discipline,
                    YearsOfExperience = discipline.YearsOfExp
                };
                result = result.Append(disc);
            }
            return result;
        }

        private async Task<Object> processDisciplineSkillChanges(IEnumerable<ResourceDisciplineResource> disciplines, string userId)
        {
            var profileDisciplines = createResourceDisciplinesFromProfile(disciplines, userId);
            var profileSkills = createResourceSkillsFromProfile(disciplines, userId);
            // Log.Logger.Information("finish getting profile Skills" + profileSkills);
            var disciplinesDB = await disciplinesRepository.GetUserDisciplines(userId);
            // Log.Logger.Information("finish getting Discipline DB");
            var skillsDB = await skillsRepository.GetUserSkills(userId);
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

        private IEnumerable<ResourceSkill> createResourceSkillsFromProfile(IEnumerable<ResourceDisciplineResource> disciplines, string userId)
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

        private IEnumerable<OutOfOffice> createOutOfOfficeFromProfile(IEnumerable<OutOfOfficeResource> availabilities, string userId)
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

        private async Task<IEnumerable<OutOfOffice>> processOutOfOfficeChanges(IEnumerable<OutOfOfficeResource> profile, string userId)
        {
            var profileAvailability = createOutOfOfficeFromProfile(profile, userId);
            var availabilityDB = await outOfOfficeRepository.GetAllOutOfOfficeForUser(userId);
            var result = Enumerable.Empty<OutOfOffice>();
            bool isSameAvail = profileAvailability.SequenceEqual(availabilityDB);
            if (!isSameAvail)
            {
                Log.Logger.Information("avail function " + isSameAvail);
                var removed = await removeAvailFromDB(profileAvailability, availabilityDB);
                var inserted = await addAvailToDB(profileAvailability, availabilityDB);

                var newOutOfOffices = await outOfOfficeRepository.GetAllOutOfOfficeForUser(userId);
                var allPositionsOfUser = await positionsRepository.GetAllPositionsOfUser(userId);
                int updatedUtilization = await utilizationRepository.CalculateUtilizationOfUser(allPositionsOfUser, newOutOfOffices);
                await usersRepository.UpdateUtilizationOfUser(updatedUtilization, userId);
                
                Log.Information("updated util {@a}", updatedUtilization);
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

        /// <summary>Search users</summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/users/search
        ///     {
        ///         "filter": {
        ///             "utilization": {
        ///                 "min": 50,
        ///                 "max": 160
        ///             },
        ///             "locations": [
        ///                 {
        ///                     "locationID": 8,
        ///                     "province": "British Columbia",
        ///                     "city": "Vancouver"
        ///                 },
        ///                 {
        ///                     "locationID": 5,
        ///                     "province": "Alberta",
        ///                     "city": "Edmonton"
        ///                 }
        ///             ],
        ///             "disciplines": {
        ///                 "Intel": [
        ///                     "Deception",
        ///                     "False Identity Creation"
        ///                 ],
        ///                 "Martial Arts": [
        ///                     "Kali"
        ///                 ],
        ///                 "Weapons": []
        ///             },
        ///             "yearsOfExps": [
        ///                 "1-3",
        ///                 "3-5",
        ///                 "10+"
        ///             ],
        ///             "startDate": "2021-10-31T00:00:00",
        ///             "endDate": "2022-02-12T00:00:00"
        ///         },
        ///         "searchWord": "e",
        ///         "orderKey": "utilization",
        ///         "order": "asc",
        ///         "page": 1
        ///     }
        ///
        /// </remarks>
        /// <param name="req"></param>
        /// <returns>
        /// Returns a list of 50 users that match provided search parameters.
        /// The key (default is utilization) is a string that determines which filter should determine the order.
        /// Order determines if it should be ascending or descending.
        /// PageNumber determines which set of users are provided.
        /// When no order/orderKey are provided, it returns the first set of 50 users sorted according to utilization in descending order. 
        /// </returns>
        /// <response code="200">Returns the requested users</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized Request</response>
        /// <response code="404">If the requested user cannot be found</response>
        /// <response code="500">Internal Server Error</response>
        [Authorize(Actions.AdminThings)]
        [HttpPost]
        [Route("users/search")]
        [ProducesResponseType(typeof(OkResponse<IEnumerable<UserSummary>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedException), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SearchUsers([FromBody] RequestSearchUsers req)
        {
            if (req == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new BadRequestException("The given user is null / Request Body cannot be read"));
            }

            try
            {
                var users = await usersRepository.GetAllUserResourcesOnFilter(req);
                if (users == null || !users.Any())
                {
                    var error = new NotFoundException("No users data found");
                    return StatusCode(StatusCodes.Status404NotFound, new CustomException<NotFoundException>(error).GetException());
                }
                var usersSummary = mapper.Map<IEnumerable<UserResource>, IEnumerable<UserSummary>>(users);
                var extra = new {
                    requestBody = req,
                    size = usersSummary.Count()
                };
                var response = new OkResponse<IEnumerable<UserSummary>>(usersSummary, "Everything is Ok", extra);
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
    }
}
