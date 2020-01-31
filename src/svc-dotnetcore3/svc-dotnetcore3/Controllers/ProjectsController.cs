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
using Newtonsoft.Json;

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
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Project>>> GetAllProjects()
        {
            var projects = await projectsRepository.GetAllProjects();
            var resource = mapper.Map<IEnumerable<Project>, IEnumerable<ProjectResource>>(projects);
            var response = new Response(resource, StatusCodes.Status200OK, "OK", "Everything is good");
            return StatusCode(StatusCodes.Status200OK, response);

            // var response = new CustomException(StatusCodes.Status404NotFound, "Not Found", "Not even there");
            // return StatusCode(StatusCodes.Status404NotFound, response);
        }

        [HttpGet]
        [Route("/projects/{projectNumber}", Name = "GetAProject")]
        [ProducesResponseType(typeof(IEnumerable<ProjectResource>), StatusCodes.Status200OK)]
        public async Task<ActionResult<Project>> GetAProject(string projectNumber)
        {
            var project = await projectsRepository.GetAProject(projectNumber);
            var resource = mapper.Map<Project, ProjectResource>(project);
            var response = new Response(resource);
            return StatusCode(StatusCodes.Status200OK, response);
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
            // string json = JsonConvert.SerializeObject(project, Formatting.Indented);
            // Console.WriteLine(json);
            var created = await projectsRepository.CreateAProject(project);
            var resource = mapper.Map<Project, ProjectResource>(created);

            // Console.WriteLine(nameof(GetAProject));
            // Console.WriteLine(this.HttpContext.Request.Scheme);
            // Console.WriteLine(this.Request.Host.ToString());

            var url = Url.Link(nameof(GetAProject), new { projectNumber = created.Number });
            Console.WriteLine(url);
            var response = new Response(resource, StatusCodes.Status201Created, default, default, new { url = url });
            return StatusCode(StatusCodes.Status201Created, response);
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