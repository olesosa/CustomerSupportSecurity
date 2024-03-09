using AutoMapper;
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
            
            var creationResult = await _userManager.CreateAsync(user, userDto.Password);

            if (creationResult.Succeeded) 
            {
                var createdUser = await _userManager.FindByEmailAsync(userDto.Email);

                var roleResult = await _userManager.AddToRoleAsync(createdUser, "Customer");

                if (!roleResult.Succeeded)
                    throw new Exception("Can not give user role");
                
                var emailResult = await _emailService.SendEmail(createdUser);

                if (!emailResult)
                {
                    _context.Users.Remove(createdUser);
                    
                    throw new Exception("Can not send an verification email");
                }
                
                return _mapper.Map<UserDto>(createdUser);
            }

            throw new Exception("Can not create user");
        }

        public async Task<bool> CheckPassword(UserLogInDto userDto, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userDto.Email);

            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<TokenResponseModel?> VerifyToken(TokenRequestDto tokenRequest)
        {
            return await _tokenGenerator.RefreshAccessToken(tokenRequest.Token, tokenRequest.RefreshToken);
        }

        public async Task<TokenResponseModel?> GetTokens(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            
            return await _tokenGenerator.GenerateTokens(user);
        }

        public async Task<bool> IsUserExist(Guid userId)
        {
            return await _context.Users.AnyAsync(u => u.Id == userId);
        }

        public async Task<bool> IsUserExist(string email)
        {
            var exist = await _context.Users.AnyAsync(u => u.Email == email);

            return exist;
        }

        public async Task<bool> VerifyEmail(Guid userId, string code)
        {
            code = code.Replace(' ', '+');
            
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            var result = await _userManager.ConfirmEmailAsync(user, code);

            return result.Succeeded;
        }
    }
}
