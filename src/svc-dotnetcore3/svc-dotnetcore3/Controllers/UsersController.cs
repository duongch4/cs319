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
using Newtonsoft.Json;

namespace Web.API.Controllers
{
    // [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUsersRepository usersRepository;
        private readonly IMapper mapper;

        public UsersController(IUsersRepository usersRepository, IMapper mapper)
        {
            this.usersRepository = usersRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        [Route("/users")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await usersRepository.GetAllUsers();
            var resource = mapper.Map<IEnumerable<User>, IEnumerable<UserResource>>(users);
            var response = new Response(resource, StatusCodes.Status200OK, "OK", "Everything is good");
            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpGet]
        [Route("/users/{username}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAUser(string username)
        {
            var user = await usersRepository.GetAUser(username);
            var resource = mapper.Map<User, UserResource>(user);
            var response = new Response(resource, StatusCodes.Status200OK, "OK", "Everything is good");
            return StatusCode(StatusCodes.Status200OK, response);
        }
    }
}