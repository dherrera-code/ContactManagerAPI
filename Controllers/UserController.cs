using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContactManagerAPI.Model.DTOs;
using ContactManagerAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ContactManagerAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AccountService _userService;
        public UserController(AccountService userService)
        {
            _userService = userService;
        }

        [HttpPost("CreateAccount")]
        public async Task<IActionResult> CreateAccount(CreateAccountDTO newUser)
        {
            var success = await _userService.CreateAccount(newUser);

            if(success) return Ok(new {success = true , message = "Account Created"});

            return BadRequest(new{success = false, message = "User creation failed! Email is already in use Or Username is already taken!"});
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO userLogin)
        {
            var success = await _userService.Login(userLogin);

            if(success != null) return Ok(new {Token = success});

            return Unauthorized(new {message = "The password you entered is incorrect. Please try again."});
        }
        [HttpPut("UpdateUsername/{id}")]
        public async Task<IActionResult> UpdateUserInfo(int id, CreateAccountDTO updateUser)
        {
            var success = await _userService.EditUsername(id, updateUser);

            if(success) return Ok(new {success});

            return BadRequest(new {Message = "Updating Username Failed! New Username may already be created!"});
        }
    }
}