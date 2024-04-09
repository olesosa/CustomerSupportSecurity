using AutoMapper;
using CS.Security.DataAccess;
using CS.Security.DTO;
using CS.Security.Helpers;
using CS.Security.Interfaces;
using CS.Security.Models;
using Microsoft.AspNetCore.Identity;

namespace CS.Security.Services;

public class AdminService : IAdminService
{
    private readonly ApplicationContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IMapper _mapper;

    public AdminService(ApplicationContext context, UserManager<User> userManager,
        RoleManager<IdentityRole<Guid>> roleManager, IMapper mapper)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _mapper = mapper;
    }

    public async Task<AdminDto> Create(AdminCreateDto adminDto)
    {
        var admin = new User()
        {
            UserName = adminDto.Email,
            Email = adminDto.Email
        };

        var creationResult = await _userManager.CreateAsync(admin, adminDto.Password);
        
        if (creationResult.Succeeded) 
        {
            var createdAdmin = await _userManager.FindByEmailAsync(adminDto.Email);
        
            var roleResult = await _userManager.AddToRoleAsync(createdAdmin, "Admin");
        
            if (!roleResult.Succeeded)
                throw new Exception("Error while giving role");
                
            return _mapper.Map<AdminDto>(createdAdmin);
        }
        
        throw new Exception("Can not create admin");
    }

    public async Task<bool> RemoveAdmin(Guid adminId)
    {
        var admin = await _userManager.FindByIdAsync(adminId.ToString());

        if (admin == null)
        {
            throw new AuthException(404, "Admin to delete not found");
        }

        var result = await _userManager.DeleteAsync(admin);

        return result.Succeeded;
    }
}