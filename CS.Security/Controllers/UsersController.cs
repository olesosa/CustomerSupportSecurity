using System.Security.Claims;
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

            if (await _userService.IsUserExist(userSignUp.Email))
            {
                return BadRequest("User already exist");
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

            if (!await _userService.IsUserExist(userLogIn.Email))
            {
                return NotFound("User does not exist");
            }

            if (!await _userService.CheckPassword(userLogIn, userLogIn.Password))
            {
                return BadRequest("Incorrect email or password");
            }

            var token = await _userService.GetTokens(userLogIn.Email);

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

            if (!await _userService.IsUserExist(userId))
            {
                return BadRequest("Invalid email parameter");
            }

            if (await _userService.VerifyEmail(userId, code)) // TODO: add email htmls
            {
                return Ok();
            }

            throw new ApiException(400, "Bad request");
        }

        [AllowAnonymous]
        [HttpPost("Token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDto tokenRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var token = await _userService.VerifyToken(tokenRequest);

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
    }
}