using CS.Security.DTO;
using CS.Security.Models;

namespace CS.Security.Interfaces;

public interface ITokenGenerator
{
    Task<TokenDto> GenerateTokens(User user);
    
    Task<TokenDto> RefreshAccessToken(string accessToken, string refreshToken);
}