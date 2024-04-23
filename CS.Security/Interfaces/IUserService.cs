using CS.Security.DTO;

namespace CS.Security.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> Create(UserSignUpDto user);
        Task<TokenDto> VerifyToken(TokenDto token);
        Task<TokenDto> GetTokens(UserLogInDto user);
        Task<bool> VerifyEmail(Guid userId, string code);
        Task<bool> Delete(string userId);
        Task<UserInfoDto> GetById(Guid userId);
    }
}
