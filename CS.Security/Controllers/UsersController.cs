﻿using System.Security.Claims;
using CS.Security.DTO;
using CS.Security.Helpers;
using CS.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CS.Security.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] UserSignUpDto userSignUp)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdUser = await _userService.Create(userSignUp);

            return Ok(createdUser);
        }

        [AllowAnonymous]
        [HttpPost("LogIn")]
        public async Task<IActionResult> LogIn([FromBody] UserLogInDto userLogIn)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var token = await _userService.GetTokens(userLogIn);

            return Ok(token);
        }

        [AllowAnonymous]
        [HttpGet("EmailVerification")]
        public async Task<IActionResult> EmailVerification([FromQuery] Guid userId, [FromQuery] string code)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _userService.VerifyEmail(userId, code))
            {
                return Ok();
            }

            throw new AuthException(400, "Bad request");
        }

        [AllowAnonymous]
        [HttpPost("Token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenDto tokenDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var token = await _userService.VerifyToken(tokenDto);

            return Ok(token);
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteUser()
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _userService.Delete(userId);

            return Ok("User is deleted");
        }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> IsUserExist([FromBody] string email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.IsUserExist(email);
            
            return Ok(result);
        }
    }
}