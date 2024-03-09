using CS.Security.DTO;
using CS.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CS.Security.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
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
                return BadRequest(ModelState);

            if (!await _userService.IsUserExist(userLogIn.Email))
                return NotFound("User does not exist");

            if (!await _userService.CheckPassword(userLogIn, userLogIn.Password))
                return BadRequest("Incorrect email or password");

            var token = await _userService.GetTokens(userLogIn.Email);

            if (token != null)
            {
                return Ok(token);
            }

            return StatusCode(StatusCodes.Status500InternalServerError, "Server error");
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
            else
            {
                return BadRequest();
            }
        }

        [AllowAnonymous]
        [HttpPost("Token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDto tokenRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var token = await _userService.VerifyToken(tokenRequest);

            if (token == null)
            {
                return BadRequest(token);
            }

            return Ok(token);
        }
    }
}
