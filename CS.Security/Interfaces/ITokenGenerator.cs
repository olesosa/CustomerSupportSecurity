using CS.Security.DTO;
using CS.Security.Models;

using CS.Security.Services.Authentication;

namespace CS.Security.Interfaces
{
    public interface ITokenGenerator
    {
        Task<string> GenerateAccessToken(User user);
        string GenerateRefreshToken(User user);
        Task<TokenDto> GenerateTokens(User user);
        Task<TokenDto> RefreshAccessToken(string accessToken, string refreshToken);

    }
}
