using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DangoAPI.Data;
using DangoAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DangoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;

        public AuthController(IAuthRepository repo)
        {
            _repo = repo;
        }

        public async Task<IActionResult> Register(string username, string password) {
            //validate request

            username = username.ToLower();
            if (await _repo.UserExists(username)) return BadRequest("Username already exists.");
            User userToCreate = new User();
            userToCreate.Username = username;
            await _repo.Register(userToCreate, password);
            return StatusCode(201);
        }

    }
}