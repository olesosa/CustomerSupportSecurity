using CS.Security.Models;
using Microsoft.AspNetCore.Identity;

namespace CS.Security.Helpers;

public class DataSeeder
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    public DataSeeder(UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task SeedAllData()
    {
        await SeedSuperAdmin();
        await SeedAdmins();
    }
    
    public async Task SeedRoles() 
    {
        if (!await _roleManager.RoleExistsAsync("User"))
            await _roleManager.CreateAsync(new IdentityRole<Guid>("User"));
        if (!await _roleManager.RoleExistsAsync("Admin"))
            await _roleManager.CreateAsync(new IdentityRole<Guid>("Admin"));
        if (!await _roleManager.RoleExistsAsync("SuperAdmin"))
            await _roleManager.CreateAsync(new IdentityRole<Guid>("SuperAdmin"));
    }

    public async Task SeedSuperAdmin()
    {
        var superAdminId = Guid.Parse("233ddc36-d493-4a74-b414-9f284c41eb61");
        
        string email = "super@gmail.com";
        string password = "P@ssword123";

        if (await _userManager.FindByIdAsync(superAdminId.ToString()) == null)
        {
            var superAdmin = new User()
            {
                Id = superAdminId,
                UserName = "SuperOles",
                Email = email,
            };
            
            var result = await _userManager.CreateAsync(superAdmin, password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(superAdmin, "SuperAdmin");
            }
        }
    }
    
    public async Task SeedAdmins()
    {
        var adminId = Guid.Parse("52c8d3a8-ebff-4309-9fb7-02b7e56a32a4");
        
        string email = "oleosad@gmail.com";
        string password = "P@ssword123";

        if (await _userManager.FindByIdAsync(adminId.ToString()) == null)
        {
            var admin = new User()
            {
                Id = adminId,
                UserName = "Oles",
                Email = email,
            };

            var result = await _userManager.CreateAsync(admin, password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
    
}
