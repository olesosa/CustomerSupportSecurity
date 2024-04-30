using CS.Security.DTO;
using CS.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CS.Security.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "SuperAdmin")]
public class AdminsController : ControllerBase
{
    private readonly IUserService _userService;

    public AdminsController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] UserSignUpDto adminDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdAdmin = await _userService.Create(adminDto, UserRoles.Admin, false);

        return Ok(createdAdmin);
    }

    [HttpDelete("{adminId:guid}")]
    public async Task<IActionResult> Remove(
        [FromRoute] Guid adminId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (await _userService.Delete(adminId.ToString()))
        {
            return Ok("Admin was deleted");
        }

        return NotFound("Admin not found");
    }

    [HttpGet]
    public async Task<IActionResult> GetAdmins()
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var admins = await _userService.GetAll(new List<string>{ UserRoles.Admin });
        
        return Ok(admins);
    }
}