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
    private readonly IAdminService _adminService;

    public AdminsController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AdminCreateDto adminDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdAdmin = await _adminService.Create(adminDto);

        return Ok(createdAdmin);
    }

    [HttpDelete("{adminId:guid}")]
    public async Task<IActionResult> Remove([FromRoute] Guid adminId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (await _adminService.RemoveAdmin(adminId))
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

        var admins = await _adminService.GetAll();
        
        return Ok(admins);
    }
}