using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fightclub.Data;
using fightclub.DTO.Auth;
using fightclub.Models;
using Microsoft.AspNetCore.Mvc;

namespace fightclub.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _auth;
        public AuthController(IAuthRepository auth)
        {
            _auth = auth;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<ServiceResponse<int>>> Register(UserRegisterDTO register)
        {
            var response = await _auth.Register(
                 new User
                 {
                     Username = register.Username
                 },
                 register.Password
             );

            if (response.Success is false)
            {
                return BadRequest(response);
            }
            return response;
        }

        [HttpPost("Login")]
        public async Task<ActionResult<ServiceResponse<string>>> Login(UserLoginDTO user)
        {
            var response = await _auth.Login(user.Username, user.Password);

            if (response.Success is false)
            {
                return BadRequest(response);
            }
            return response;

        }


    }
}