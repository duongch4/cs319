using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.API.Application.Models;
using Web.API.Application.Repository;

namespace Web.API.Controllers
{
    [Authorize]
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
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            var response = await usersRepository.GetAllUsers();
            var viewModel = mapper.Map<IEnumerable<User>>(response);
            return Ok(viewModel);
        }

        [HttpGet]
        [Route("/users/{username}")]
        public async Task<ActionResult<User>> GetAUser(string username)
        {
            var response = await usersRepository.GetAUser(username);
            var viewModel = mapper.Map<User>(response);
            return Ok(viewModel);
        }
    }
}