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
    [Authorize]
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
        public async Task<IActionResult> GetAllProjects()
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAProject(string projectNumber)
        {
            var project = await projectsRepository.GetAProject(projectNumber);
            var resource = mapper.Map<Project, ProjectResource>(project);
            var response = new Response(resource);
            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpGet]
        [Route("/projects/most-recent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMostRecentProjects()
        {
            var mostRecentProjects = await projectsRepository.GetMostRecentProjects();
            var resource = mapper.Map<IEnumerable<Project>, IEnumerable<ProjectResource>>(mostRecentProjects);
            var response = new Response(resource);
            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpPost]
        [Route("/projects")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAProject([FromBody] Project project)
        {
            string json = JsonConvert.SerializeObject(project, Formatting.Indented);
            Console.WriteLine(json);
            var created = await projectsRepository.CreateAProject(project);
            var resource = mapper.Map<Project, ProjectResource>(created);

            // Console.WriteLine(nameof(GetAProject));
            // Console.WriteLine(this.HttpContext.Request.Scheme);
            // Console.WriteLine(this.Request.Host.ToString());

            var url = Url.Link(nameof(GetAProject), new { projectNumber = created.Number });
            Console.WriteLine(url);
            var response = new Response(resource, StatusCodes.Status201Created, "OK", "Successfully created", new { url = url });
            return StatusCode(StatusCodes.Status201Created, response);
        }

        [HttpPut]
        [Route("/projects")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAProject([FromBody] Project project)
        {
            var updated = await projectsRepository.UpdateAProject(project);
            var resource = mapper.Map<Project, ProjectResource>(updated);
            var response = new Response(resource, StatusCodes.Status200OK, "OK", "Successfully updated");
            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpDelete]
        [Route("/projects/{number}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteAProject([FromRoute] string number)
        {
            var deleted = await projectsRepository.DeleteAProject(number);
            var resource = mapper.Map<Project, ProjectResource>(deleted);
            var response = new Response(resource, StatusCodes.Status200OK, "OK", "Successfully deleted");
            return StatusCode(StatusCodes.Status200OK, response);
        }
    }
}