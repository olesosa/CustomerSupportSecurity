using CS.Security.DTO;

namespace CS.Security.Interfaces;

public interface IUserService
{
    Task<UserDto> Create(UserSignUpDto user, string role, bool sendInvitationEmail);
    Task<TokenDto> VerifyToken(TokenDto token);
    Task<TokenDto> GetTokens(UserLogInDto user);
    Task<bool> VerifyEmail(Guid userId, string code);
    Task<bool> Delete(string userId);
    Task<UserInfoDto> GetById(Guid userId);
    Task<List<UserInfoDto>> GetAll(List<string> roleNames);
}