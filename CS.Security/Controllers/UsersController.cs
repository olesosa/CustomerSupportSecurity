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
        private readonly string _url = Environments.FrontAddress;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp(
            [FromBody] UserSignUpDto userSignUp)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdUser = await _userService.Create(userSignUp, UserRoles.User, true);

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

            if (!await _userService.VerifyEmail(userId, code))
            {
                throw new AuthException(400, "Bad request");
            }

            return Redirect($"{_url}login");
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
        [HttpGet]
        public async Task<IActionResult> GetUserInfo()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

            var user = await _userService.GetById(userId);
            
            return Ok(user);
        }
    }
}