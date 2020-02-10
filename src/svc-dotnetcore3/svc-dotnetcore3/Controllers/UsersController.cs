using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.API.Application.Models;
using Web.API.Application.Repository;
using Microsoft.Extensions.Configuration;
using System;

namespace Web.API.Controllers
{
    // [Authorize]
    public class UsersController : ControllerBase
    {
        public IConfiguration Configuration { get; }
        private readonly IUsersRepository usersRepository;
        private readonly IMapper mapper;

        public UsersController(IUsersRepository usersRepository, IMapper mapper, IConfiguration configuration)
        {
            this.usersRepository = usersRepository;
            this.mapper = mapper;
            this.Configuration = configuration;
        }

        [HttpGet]
        [Route("/users")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            // var response = await usersRepository.GetAllUsers();
            // var viewModel = mapper.Map<IEnumerable<User>>(response);
            // return Ok(viewModel);
            try
            {
                var authSettings = Configuration.GetSection("AzureAd");

                string[] payload = { authSettings.GetValue<string>("Authority"), authSettings.GetValue<string>("ClientId"), authSettings.GetValue<string>("Tenant") };
                return Ok(new { payload = payload });
            }
            catch (Exception err)
            {
                var errMessage = $"Source: {err.Source}\n  Message: {err.Message}\n  StackTrace: {err.StackTrace}\n";
                return BadRequest(new { payload = errMessage });
            }
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