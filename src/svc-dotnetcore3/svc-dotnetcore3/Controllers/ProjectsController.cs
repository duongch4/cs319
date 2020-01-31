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

namespace Web.API.Controllers
{
    // [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectsRepository projectsRepository;
        private readonly IMapper mapper;

        public ProjectsController(IProjectsRepository projectsRepository, IMapper mapper)
        {
            this.projectsRepository = projectsRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        [Route("/projects")]
        [ProducesResponseType(typeof(IEnumerable<ProjectResource>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Project>>> GetAllProjects()
        {
            var projects = await projectsRepository.GetAllProjects();
            var resource = mapper.Map<IEnumerable<Project>, IEnumerable<ProjectResource>>(projects);
            var response = new Response(StatusCodes.Status200OK, "OK", resource, "Everything is good");
            return StatusCode(StatusCodes.Status200OK, response);
            
            // var response = new CustomException(StatusCodes.Status404NotFound, "Not Found", "Not even there");
            // return StatusCode(StatusCodes.Status404NotFound, response);
        }

        [HttpGet]
        [Route("/projects/{projectNumber}", Name = "GetAProject")]
        public async Task<ActionResult<Project>> GetAProject(string projectNumber)
        {
            var response = await projectsRepository.GetAProject(projectNumber);
            var viewModel = mapper.Map<Project>(response);
            return Ok(viewModel);
        }

        [HttpGet]
        [Route("/projects/most-recent")]
        public async Task<ActionResult<IEnumerable<Project>>> GetMostRecentProjects()
        {
            var response = await projectsRepository.GetMostRecentProjects();
            var viewModel = mapper.Map<IEnumerable<Project>>(response);
            return Ok(viewModel);
        }

        [HttpPost]
        [Route("/projects")]
        public async Task<ActionResult<Project>> CreateAProject([FromBody] Project project)
        {
            var response = await projectsRepository.CreateAProject(project);
            var viewModel = mapper.Map<Project>(response);
            return Created("GetAProject", viewModel);
        }

        [HttpPut]
        [Route("/projects")]
        public async Task<ActionResult<Project>> UpdateAProject([FromBody] Project project)
        {
            var response = await projectsRepository.UpdateAProject(project);
            var viewModel = mapper.Map<Project>(response);
            return Ok(viewModel);
        }

        [HttpDelete]
        [Route("/projects/{number}")]
        public async Task<ActionResult<Project>> DeleteAProject([FromRoute] string number)
        {
            var response = await projectsRepository.DeleteAProject(number);
            var viewModel = mapper.Map<Project>(response);
            return Ok(viewModel);
        }
    }
}