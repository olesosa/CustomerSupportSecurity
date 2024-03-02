using CS.Security.DTO;
using CS.Security.Html;
using CS.Security.Interfaces;
using CS.Security.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto.Fpe;

namespace CS.Security.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        readonly IUserService _userService;
        readonly ITokenGenerator _tokenGenerator;
        readonly IEmailService _emailService;
        readonly UserManager<User> _userManager;

        public UserController(IUserService userService, IEmailService emailService, 
            ITokenGenerator tokenGenerator, UserManager<User> userManager)
        {
            _userService = userService;
            _emailService = emailService;
            _tokenGenerator = tokenGenerator;
            _userManager = userManager;
        }

        [AllowAnonymous]
        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] UserSignUpDto userSignUp)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (await _userManager.FindByEmailAsync(userSignUp.Email) != null)
                {
                    return BadRequest("User already exist");
                }

                if (await _userService.Create(userSignUp))
                {
                    var createdUser = await _userService.GetByEmail(userSignUp.Email);

                    if (await _emailService.SendEmail(createdUser))
                    {
                        return Ok("Email verify has been sent");
                    }
                    else
                        return StatusCode(StatusCodes.Status500InternalServerError, "Somth went wrong with sending email");
                }

            }
            catch { }

            return StatusCode(StatusCodes.Status500InternalServerError, "Server error");
        }

        [AllowAnonymous]
        [HttpPost("LogIn")]
        public async Task<IActionResult> LogIn([FromBody] UserLogInDto userLogIn)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _userManager.FindByEmailAsync(userLogIn.Email);

                if (user == null)
                {
                    return NotFound("User does not exist");
                }

                var token = await _tokenGenerator.GenerateTokens(user);

                if (token == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Token error");
                }

                return Ok(token);
            }
            catch { }

            return StatusCode(StatusCodes.Status500InternalServerError, "Server error");
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> VerifyEmail([FromQuery] Guid userId, [FromQuery] string code)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.GetById(userId);

            if (user == null)
            {
                return BadRequest("Invalid email parameter");
            }

            code = code.Replace(' ', '+');

            if (await _emailService.VerifyEmail(user, code)) 
            {
                return Content(EmailHtmls.SuccessEmail, "text/html");
            }
            else
            {
                return Content(EmailHtmls.FailEmail, "text/html");
            }
        }

        [AllowAnonymous]
        [HttpPost]
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
