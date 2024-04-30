using AutoMapper;
using CS.Security.DataAccess;
using CS.Security.DTO;
using CS.Security.Helpers;
using CS.Security.Interfaces;
using CS.Security.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CS.Security.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public UserService(
            ApplicationContext context,
            UserManager<User> userManager, 
            ITokenGenerator tokenGenerator,
            RoleManager<IdentityRole<Guid>> roleManager,
            IMapper mapper,
            IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _tokenGenerator = tokenGenerator;
            _mapper = mapper;
            _emailService = emailService;
            _roleManager = roleManager;
        }

        public async Task<UserDto> Create(UserSignUpDto userDto, string role, bool sendInvitationEmail)
        {
            var user = _mapper.Map<User>(userDto);

            var exist = await _context.Users.AnyAsync(u => u.Email == user.Email);

            if (exist)
            {
                throw new AuthException(400, "User already exist");
            }
            
            var creationResult = await _userManager.CreateAsync(user, userDto.Password);
            if (!creationResult.Succeeded)
            {
                throw new AuthException(500, "Can not create user");
            }

            var createdUser = await _userManager.FindByEmailAsync(userDto.Email);

            var roleResult = await _userManager.AddToRoleAsync(createdUser, role);

            if (!roleResult.Succeeded)
            {
                throw new AuthException(500,"Error while giving role");
            }

            if (!sendInvitationEmail)
            {
                return _mapper.Map<UserDto>(createdUser);
            }
                
            var emailResult = await _emailService.SendEmail(createdUser);

            if (emailResult)
            {
                return _mapper.Map<UserDto>(createdUser);
            }
                
            _context.Users.Remove(createdUser);
                        
            throw new AuthException(500,"Can not send an verification email");

        }

        public async Task<TokenDto> VerifyToken(TokenDto token)
        {
            var response = await _tokenGenerator.RefreshAccessToken(token.Token, token.RefreshToken);

            if (response == null)
            {
                throw new AuthException(400, "Response generation error");
            }

            return response;
        }

        public async Task<TokenDto> GetTokens(UserLogInDto userDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userDto.Email);

            if (user == null)
            {
                throw new AuthException(400, "User doesnt exist");
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                throw new AuthException(401, "Email is not confirmed");
            }

            if (!await _userManager.CheckPasswordAsync(user, userDto.Password))
            {
                throw new AuthException(400, "Incorrect email or password");
            }

            return await _tokenGenerator.GenerateTokens(user);
        }

        public async Task<bool> VerifyEmail(Guid userId, string code)
        {
            var exist = await _context.Users.AnyAsync(u => u.Id == userId);

            if (!exist)
            {
                throw new AuthException(400, "User doesnt exist");
            }
            
            code = code.Replace(' ', '+');
            
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new AuthException(404, "User does not exist");
            }
            
            var result = await _userManager.ConfirmEmailAsync(user, code);

            return result.Succeeded;
        }

        public async Task<bool> Delete(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new AuthException(404, "User not found");
            }
            
            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                throw new AuthException(500, "Can not delete user");
            }
            
            return result.Succeeded;
        }

        public async Task<UserInfoDto> GetById(Guid userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new AuthException(404, "User not found");
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            return new UserInfoDto(
                user.Id,
                user.UserName,
                user.Email,
                userRoles.First());
        }

        public async Task<List<UserInfoDto>> GetAll(List<string> roleNames)
        {
            var getRoleTasks = roleNames
                .Select(roleName => _roleManager.FindByNameAsync(roleName));
            
            var roles = await Task.WhenAll(getRoleTasks);
            var getUserTasks = roles.Select(role => _userManager.GetUsersInRoleAsync(role.Name));
            var users = await Task.WhenAll(getUserTasks);

            return users
                .Select(_mapper.Map<UserInfoDto>)
                .ToList();
        }
    }
}
