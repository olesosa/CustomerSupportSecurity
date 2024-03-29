using CS.Security.Models;
using CS.Security.Services.Authentication;

namespace CS.Security.Interfaces
{
    public interface ITokenGenerator
    {
        Task<string> GenerateAccessToken(User user);
        string GenerateRefreshToken(User user);
        Task<TokenResponseModel> GenerateTokens(User user);
        Task<TokenResponseModel> RefreshAccessToken(string accessToken, string refreshToken);

    }
}
