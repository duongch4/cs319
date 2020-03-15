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

namespace Web.API.Controllers {
    [Authorize]
    [Route("api")]
    [Produces("application/json")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class AdminController : ControllerBase {
        private readonly ILocationsRepository locationsRepository;
        private readonly IDisciplinesRepository disciplinesRepository;
        private readonly ISkillsRepository skillsRepository;
        private readonly IMapper mapper;

        public AdminController(ILocationsRepository locationsRepository, IDisciplinesRepository disciplinesRepository, ISkillsRepository skillsRepository, IMapper mapper)
        {
            this.locationsRepository = locationsRepository;
            this.disciplinesRepository = disciplinesRepository;
            this.skillsRepository = skillsRepository;
            this.mapper = mapper;
        }

        /// <summary>Create a new discipline</summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/admin/disciplines
        ///
        /// </remarks>
        /// <param name="discipline"></param>
        /// <returns>The ID of the newly created discipline</returns>
        /// <response code="200">Returns the discipline ID</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized Request</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPost]
        [Route("admin/disciplines")]
        [ProducesResponseType(typeof(CreatedResponse<int>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedException), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateADiscipline([FromBody] DisciplineResource discipline) {
            if (discipline == null)
            {
                var error = new BadRequestException("The given discipline is null / Request Body cannot be read");
                return StatusCode(StatusCodes.Status400BadRequest, new CustomException<BadRequestException>(error).GetException());
            }

            try
            {
                var createdDisciplineID = await disciplinesRepository.CreateADiscipline(discipline);
                var response = new CreatedResponse<int>(createdDisciplineID, $"Successfully created discipline '{createdDisciplineID}'");
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

        // [HttpPut]
        // [Route("admin/disciplines/{disciplineID}")]
        // [ProducesResponseType(typeof(UpdatedResponse<string>), StatusCodes.Status200OK)]
        // [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        // [ProducesResponseType(typeof(UnauthorizedException), StatusCodes.Status401Unauthorized)]
        // [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        // public async Task<IActionResult> UpdateADiscipline([FromBody] DisciplineResource discipline, int disciplineID) {
        //     // Log.Information("{@a}", disciplineID);
        //     if(discipline == null)
        //     {
        //         return StatusCode(StatusCodes.Status400BadRequest, new BadRequestException("The given discipline is null / Request Body cannot be read"));
        //     }
        //     if(disciplineID != discipline.Id) {
        //         return StatusCode(StatusCodes.Status400BadRequest, new BadRequestException("Provided IDs conflict"));
        //     }
        //     try {
        //         var updated = await disciplinesRepository.UpdateADiscipline(discipline);
        //         if (updated == -1)
        //         {
        //             var errMessage = $"Query returns failure status on updating discipline '{disciplineID}'";
        //             return StatusCode(StatusCodes.Status500InternalServerError, new InternalServerException(errMessage));
        //         }
        //         var response = new UpdatedResponse<int>(updated, "Successfully updated");
        //         return StatusCode(StatusCodes.Status200OK, response);
        //     } catch (Exception err) {
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

        /// <summary>Delete a discipline</summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/admin/disciplines/2
        ///
        /// </remarks>
        /// <param name="disciplineID"></param>
        /// <returns>The ID of the deleted discipline</returns>
        /// <response code="200">Returns the ID of the deleted discipline</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized Request</response>
        /// <response code="404">If no disciplines are found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpDelete]
        [Route("admin/disciplines/{disciplineID}")]
        [ProducesResponseType(typeof(DeletedResponse<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedException), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteADiscipline([FromRoute] int disciplineID) {
            if (disciplineID == 0)
            {
                var error = new BadRequestException("The given discipline ID is invalid");
                return StatusCode(StatusCodes.Status400BadRequest, new CustomException<BadRequestException>(error).GetException());
            }

            try
            {
                var deleted = await disciplinesRepository.DeleteADiscipline(disciplineID);
                if (deleted == null)
                {
                    var error = new NotFoundException("The given discipline cannot be found on database");
                    return StatusCode(StatusCodes.Status404NotFound, new CustomException<NotFoundException>(error).GetException());
                }
                var response = new DeletedResponse<int>(deleted.Id, $"Successfully deleted discipline '{deleted.Id}'");
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

        /// <summary>Create a new skill for discipline</summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/admin/disciplines/3/skills
        ///
        /// </remarks>
        /// <param name="disciplineID"></param>
        /// <param name="skill"></param>
        /// <returns>The ID of the newly created skill</returns>
        /// <response code="200">Returns the skill ID</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized Request</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPost]
        [Route("admin/disciplines/{disciplineID}/skills")]
        [ProducesResponseType(typeof(CreatedResponse<int>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedException), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateASkill([FromBody] DisciplineSkillResource skill, int disciplineID) {
            if (skill == null)
            {
                var error =  new BadRequestException("The given skill is null / Request Body cannot be read");
                return StatusCode(StatusCodes.Status400BadRequest, new CustomException<BadRequestException>(error).GetException());
            }

            if (disciplineID == 0 || disciplineID != skill.DisciplineId)
            {
                var error = new BadRequestException("The given discipline is invalid / Request Body cannot be read");
                return StatusCode(StatusCodes.Status400BadRequest, new CustomException<BadRequestException>(error).GetException());
            }

            try
            {
                var createdSkillID = await skillsRepository.CreateASkill(skill);
                var response = new CreatedResponse<int>(createdSkillID, $"Successfully created skill '{createdSkillID}' associated with discipline '{disciplineID}'");
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

        /// <summary>Delete a skill for given discipline</summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/admin/disciplines/2/skills/Kali
        ///
        /// </remarks>
        /// <param name="disciplineID"></param>
        /// <param name="skillName"></param>
        /// <returns>The ID of the deleted skill</returns>
        /// <response code="200">Returns the ID of the deleted skill</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized Request</response>
        /// <response code="404">If no disciplines are found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpDelete]
        [Route("admin/disciplines/{disciplineID}/skills/{skillName}")]
        [ProducesResponseType(typeof(DeletedResponse<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedException), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteASkill([FromRoute] int disciplineID, [FromRoute] string skillName) {
            if (skillName == null)
            {
                var error = new BadRequestException("The given skill is invalid");
                return StatusCode(StatusCodes.Status400BadRequest, new CustomException<BadRequestException>(error).GetException());
            }

            if (disciplineID == 0)
            {
                var error = new BadRequestException("The given discipline ID is invalid");
                 return StatusCode(StatusCodes.Status400BadRequest, new CustomException<BadRequestException>(error).GetException());
            }

            try
            {
                var deleted = await skillsRepository.DeleteASkill(skillName, disciplineID);
                if (deleted == null)
                {
                    var error = new NotFoundException("The given skill cannot be found on database");
                    return StatusCode(StatusCodes.Status404NotFound, new CustomException<NotFoundException>(error).GetException());
                }

                var response = new DeletedResponse<int>(deleted.Id, $"Successfully deleted skill '{deleted.Id}'");
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

        /// <summary>Create a new location</summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/admin/locations
        ///
        /// </remarks>
        /// <param name="location"></param>
        /// <returns>The ID of the newly created location</returns>
        /// <response code="200">Returns the location ID</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized Request</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPost]
        [Route("admin/locations")]
        [ProducesResponseType(typeof(CreatedResponse<int>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedException), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateALocation([FromBody] LocationResource location) {
            if (location == null)
            {
                var error = new BadRequestException("The given location is null / Request Body cannot be read");
                return StatusCode(StatusCodes.Status400BadRequest, new CustomException<BadRequestException>(error).GetException());
            }

            try
            {
                var createdLocationID = await locationsRepository.CreateALocation(location);
                var response = new CreatedResponse<int>(createdLocationID, $"Successfully created location '{createdLocationID}'");
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

        // [HttpPut]
        // [Route("admin/locations/{locationID}")]
        // [ProducesResponseType(typeof(UpdatedResponse<string>), StatusCodes.Status200OK)]
        // [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        // [ProducesResponseType(typeof(UnauthorizedException), StatusCodes.Status401Unauthorized)]
        // [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        // public async Task<IActionResult> UpdateALocation([FromBody] LocationResource location, int locationID) {}

        /// <summary>Delete a location</summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/admin/locations/2
        ///
        /// </remarks>
        /// <param name="locationID"></param>
        /// <returns>The ID of the deleted location</returns>
        /// <response code="200">Returns the ID of the deleted location</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized Request</response>
        /// <response code="404">If no locations are found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpDelete]
        [Route("admin/locations/{locationID}")]
        [ProducesResponseType(typeof(DeletedResponse<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedException), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteALocation([FromRoute] int locationID) {
            if (locationID == 0)
            {
                var error = new BadRequestException("The given location ID is invalid");
                return StatusCode(StatusCodes.Status400BadRequest, new CustomException<BadRequestException>(error).GetException());
            }

            try
            {
                var deleted = await locationsRepository.DeleteALocation(locationID);
                if (deleted == null)
                {
                    var error = new NotFoundException("The given location cannot be found on database");
                    return StatusCode(StatusCodes.Status404NotFound, new CustomException<NotFoundException>(error).GetException());
                }
                var response = new DeletedResponse<int>(deleted.Id, $"Successfully deleted location '{deleted.Id}'");
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
    
        /// <summary>Create a new province</summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/admin/provinces
        ///
        /// </remarks>
        /// <param name="location"></param>
        /// <returns>The name of the newly created province</returns>
        /// <response code="200">Returns the province name</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized Request</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPost]
        [Route("admin/provinces")]
        [ProducesResponseType(typeof(CreatedResponse<string>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedException), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateAProvince([FromBody] LocationResource location) {
            if (location == null)
            {
                var error = new BadRequestException("The given province is null / Request Body cannot be read");
                return StatusCode(StatusCodes.Status400BadRequest, new CustomException<BadRequestException>(error).GetException());
            }

            try
            {
                var createdProvinceName = await locationsRepository.CreateAProvince(location.Province);
                var response = new CreatedResponse<string>(createdProvinceName, $"Successfully created province '{createdProvinceName}'");
                Log.Information("@{a}", response);
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
                    Log.Information("second bad request");
                    var error = new BadRequestException(errMessage);
                    return StatusCode(StatusCodes.Status400BadRequest, new CustomException<BadRequestException>(error).GetException());
                }
            }
        }

        /// <summary>Delete a province</summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/admin/provinces/Alberta
        ///
        /// </remarks>
        /// <param name="province"></param>
        /// <returns>The name of the deleted province</returns>
        /// <response code="200">Returns the name of the deleted province</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized Request</response>
        /// <response code="404">If no provinces are found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpDelete]
        [Route("admin/provinces/{province}")]
        [ProducesResponseType(typeof(DeletedResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedException), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAProvince([FromRoute] string province) {
            if (province == null)
            {
                var error = new BadRequestException("The given province is invalid");
                return StatusCode(StatusCodes.Status400BadRequest, new CustomException<BadRequestException>(error).GetException());
            }

            try
            {
                var deleted = await locationsRepository.DeleteAProvince(province);
                if (deleted == null)
                {
                    var error = new NotFoundException("The given province cannot be found on database");
                    return StatusCode(StatusCodes.Status404NotFound, new CustomException<NotFoundException>(error).GetException());
                }
                var response = new DeletedResponse<string>(deleted, $"Successfully deleted location '{deleted}'");
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