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

        [HttpPost]
        [Route("admin/disciplines")]
        [ProducesResponseType(typeof(CreatedResponse<string>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedException), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateADiscipline([FromBody] DisciplineResource discipline) {
            if (discipline == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new BadRequestException("The given discipline is null / Request Body cannot be read"));
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
                    return StatusCode(StatusCodes.Status500InternalServerError, error);
                }
                else
                {
                    var error = new BadRequestException(errMessage);
                    return StatusCode(StatusCodes.Status400BadRequest, error);
                }
            }
        }

        [HttpPut]
        [Route("admin/disciplines/{disciplineID}")]
        [ProducesResponseType(typeof(UpdatedResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedException), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateADiscipline([FromBody] DisciplineResource discipline, int disciplineID) {
            // Log.Information("{@a}", disciplineID);
            if(discipline == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new BadRequestException("The given discipline is null / Request Body cannot be read"));
            }
            if(disciplineID != discipline.Id) {
                return StatusCode(StatusCodes.Status400BadRequest, new BadRequestException("Provided IDs conflict"));
            }
            try {
                var updated = await disciplinesRepository.UpdateADiscipline(discipline);
                if (updated == -1)
                {
                    var errMessage = $"Query returns failure status on updating discipline '{disciplineID}'";
                    return StatusCode(StatusCodes.Status500InternalServerError, new InternalServerException(errMessage));
                }
                var response = new UpdatedResponse<int>(updated, "Successfully updated");
                return StatusCode(StatusCodes.Status200OK, response);
            } catch (Exception err) {
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

        [HttpDelete]
        [Route("admin/disciplines/{disciplineID}")]
        [ProducesResponseType(typeof(DeletedResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedException), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteADiscipline([FromRoute] int disciplineID) {
            if (disciplineID == 0)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new BadRequestException("The given discipline ID is invalid"));
            }

            try
            {
                var deleted = await disciplinesRepository.DeleteADiscipline(disciplineID);
                if (deleted == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new NotFoundException("The given discipline cannot be found on database"));
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
                    return StatusCode(StatusCodes.Status500InternalServerError, error);
                }
                else
                {
                    var error = new BadRequestException(errMessage);
                    return StatusCode(StatusCodes.Status400BadRequest, error);
                }
            }
        }

        [HttpPost]
        [Route("admin/disciplines/{disciplineID}/skills")]
        [ProducesResponseType(typeof(CreatedResponse<string>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedException), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateASkill([FromBody] DisciplineSkillResource skill, int disciplineID) {
            if (skill == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new BadRequestException("The given skill is null / Request Body cannot be read"));
            }

            if (disciplineID == 0 || disciplineID != skill.DisciplineId)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new BadRequestException("The given discipline is invalid / Request Body cannot be read"));
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
                    return StatusCode(StatusCodes.Status500InternalServerError, error);
                }
                else
                {
                    var error = new BadRequestException(errMessage);
                    return StatusCode(StatusCodes.Status400BadRequest, error);
                }
            }
        }

        // [HttpPut]
        // [Route("admin/disciplines/{disciplineID}/skills/{skillID}")]
        // [ProducesResponseType(typeof(UpdatedResponse<string>), StatusCodes.Status200OK)]
        // [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        // [ProducesResponseType(typeof(UnauthorizedException), StatusCodes.Status401Unauthorized)]
        // [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        // public async Task<IActionResult> UpdateASkill([FromBody] DisciplineSkillResource skill, int disciplineID, int skillID) {}

        [HttpDelete]
        [Route("admin/disciplines/{disciplineID}/skills/{skillName}")]
        [ProducesResponseType(typeof(DeletedResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedException), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteASkill([FromRoute] int disciplineID, [FromRoute] string skillName) {
            if (skillName == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new BadRequestException("The given skill is invalid"));
            }

            if (disciplineID == 0)
            {
                 return StatusCode(StatusCodes.Status400BadRequest, new BadRequestException("The given discipline ID is invalid"));
            }

            try
            {
                var deleted = await skillsRepository.DeleteASkill(skillName, disciplineID);
                if (deleted == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new NotFoundException("The given skill cannot be found on database"));
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
                    return StatusCode(StatusCodes.Status500InternalServerError, error);
                }
                else
                {
                    var error = new BadRequestException(errMessage);
                    return StatusCode(StatusCodes.Status400BadRequest, error);
                }
            }
        }

        [HttpPost]
        [Route("admin/locations")]
        [ProducesResponseType(typeof(CreatedResponse<string>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedException), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateALocation([FromBody] LocationResource location) {
            if (location == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new BadRequestException("The given location is null / Request Body cannot be read"));
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
                    return StatusCode(StatusCodes.Status500InternalServerError, error);
                }
                else
                {
                    var error = new BadRequestException(errMessage);
                    return StatusCode(StatusCodes.Status400BadRequest, error);
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

        [HttpDelete]
        [Route("admin/locations/{locationID}")]
        [ProducesResponseType(typeof(DeletedResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedException), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(InternalServerException), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteALocation([FromRoute] int locationID) {
            if (locationID == 0)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new BadRequestException("The given location ID is invalid"));
            }

            try
            {
                var deleted = await locationsRepository.DeleteALocation(locationID);
                if (deleted == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new NotFoundException("The given location cannot be found on database"));
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
}