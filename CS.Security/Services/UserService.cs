using AutoMapper;
using CS.Security.DataAccess;
using CS.Security.DTO;
using CS.Security.Helpers;
using CS.Security.Interfaces;
using CS.Security.Models;
using CS.Security.Services.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CS.Security.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public UserService(ApplicationContext context, UserManager<User> userManager, 
            ITokenGenerator tokenGenerator, IMapper mapper, IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _tokenGenerator = tokenGenerator;
            _mapper = mapper;
            _emailService = emailService;
        }

        public async Task<UserDto> Create(UserSignUpDto userDto)
        {
            var user = _mapper.Map<User>(userDto);

            var exist = await _context.Users.AnyAsync(u => u.Email == user.Email);

            if (exist)
            {
                throw new AuthException(400, "User already exist");
            }
            
            var creationResult = await _userManager.CreateAsync(user, userDto.Password);

            if (creationResult.Succeeded) 
            {
                var createdUser = await _userManager.FindByEmailAsync(userDto.Email);

                var roleResult = await _userManager.AddToRoleAsync(createdUser, "User");

                if (!roleResult.Succeeded)
                {
                    throw new AuthException(500,"Error while giving role");
                }
                
                var emailResult = await _emailService.SendEmail(createdUser);

                if (!emailResult)
                {
                    _context.Users.Remove(createdUser);
                    
                    throw new AuthException(500,"Can not send an verification email");
                }
                
                return _mapper.Map<UserDto>(createdUser);
            }

            throw new AuthException(500,"Can not create user");
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

            if (!await _userManager.CheckPasswordAsync(user, userDto.Password))
            {
                throw new AuthException(400, "Incorrect email or password");
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                throw new AuthException(401, "Email is not confirmed");
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
            
            var result = _userManager.DeleteAsync(user).IsCompletedSuccessfully;

            if (!result)
            {
                throw new AuthException(500, "Can not delete user");
            }
            
            return result;
        }

        public async Task<UserDto> GetById(Guid userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new AuthException(404, "User not found");
            }

            return _mapper.Map<UserDto>(user);
        }

        public async Task<bool> IsUserExist(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            return user != null;
        }
    }
}
