using CS.Security.DTO;
using CS.Security.Models;

using CS.Security.Services.Authentication;//not used

namespace CS.Security.Interfaces
{
    public interface ITokenGenerator
    {
        Task<string> GenerateAccessToken(User user);//make it private
        string GenerateRefreshToken(User user);//make it private
        Task<TokenDto> GenerateTokens(User user);
        Task<TokenDto> RefreshAccessToken(string accessToken, string refreshToken);

    }
}
