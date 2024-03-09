using CS.Security.DTO;
using CS.Security.Models;
using CS.Security.Servises.Authentication;

namespace CS.Security.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> Create(UserSignUpDto user);
        Task<bool> CheckPassword(UserLogInDto user, string password);
        Task<TokenResponseModel?> VerifyToken(TokenRequestDto tokenRequest);
        Task<TokenResponseModel?> GetTokens(string email);
        Task<bool> IsUserExist(Guid userId);
        Task<bool> IsUserExist(string email);
        Task<bool> VerifyEmail(Guid userId, string code);
    }
}
