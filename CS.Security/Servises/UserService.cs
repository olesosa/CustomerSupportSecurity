using CS.Security.DataAccess;
using CS.Security.DTO;
using CS.Security.Interfaces;
using CS.Security.Models;
using CS.Security.Servises.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CS.Security.Servises
{
    public class UserService : IUserService
    {
        readonly ApplicationContext _context;
        readonly UserManager<User> _userManager;
        readonly RoleManager<IdentityRole<Guid>> _roleManager;
        readonly ITokenGenerator _tokenGenerator;

        public UserService(ApplicationContext context, UserManager<User> userManager, 
            RoleManager<IdentityRole<Guid>> roleManager, ITokenGenerator tokenGenerator)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<bool> Create(UserSignUpDto user)
        {
            var userToSignUp = new User()
            {
                UserName = user.Name,
                Email = user.Email,
            };

            var creationResult = await _userManager.CreateAsync(userToSignUp, user.Password);

            if (creationResult.Succeeded) 
            {
                var createdUser = await _userManager.FindByEmailAsync(user.Email);

                var roleResult = await _userManager.AddToRoleAsync(createdUser, "Customer");

                return roleResult.Succeeded;
            }
            else
            {
                return false;
            }
        }

        public async Task<User?> GetByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u=>u.Email == email); 
        }

        public async Task<User?> GetById(Guid userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<TokenResponseModel?> VerifyToken(TokenRequestDto tokenRequest)
        {
            var token = await _tokenGenerator.RefreshAccessToken(tokenRequest.Token, tokenRequest.RefreshToken);
        }
    }
}
