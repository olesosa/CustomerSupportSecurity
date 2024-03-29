using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CS.Security.Helpers;
using CS.Security.Interfaces;
using CS.Security.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace CS.Security.Services.Authentication
{
    public class TokenGenerator : ITokenGenerator
    {
        private const int RefreshTokenSize = 32;

        private readonly UserManager<User> _userManager;
        private readonly AuthSettings _authSettings;

        private const string EmailConfirmedClaimKey = "EmailConfirmed";

        public TokenGenerator(UserManager<User> userManager, AuthSettings authSettings)
        {
            _userManager = userManager;
            _authSettings = authSettings;
        }

        public async Task<string> GenerateAccessToken(User user)
        {
            var handler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_authSettings.SecretKey);
            var roles = await _userManager.GetRolesAsync(user);

            ClaimsIdentity identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
                new Claim(EmailConfirmedClaimKey, user.EmailConfirmed.ToString())
            });

            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _authSettings.Issuer,
                Audience = _authSettings.Audience,
                SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Subject = identity,
                Expires = DateTime.Now.AddMinutes(_authSettings.AccessTokenExpirationMinutes)
            });
            return handler.WriteToken(securityToken);
        }

        public string GenerateRefreshToken(User user)
        {
            var randomNumber = new byte[RefreshTokenSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public async Task<TokenResponseModel> GenerateTokens(User user)
        {
            user.RefreshToken = GenerateRefreshToken(user);
            user.ExpirationTime = DateTimeOffset.UtcNow.AddSeconds(_authSettings.AccessTokenExpirationMinutes)
                .ToUnixTimeSeconds();
            // DateTimeOffset.FromUnixTimeSeconds(user.ExpirationTime);

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                throw new Exception("Unable to create refresh token");
            }

            return new TokenResponseModel
            {
                AccessToken = await GenerateAccessToken(user),
                RefreshToken = user.RefreshToken,
            };
        }

        public async Task<TokenResponseModel> RefreshAccessToken(string accessToken, string refreshToken)
        {
            var principal = GetPrincipalFromExpiredToken(accessToken);
            var user = await _userManager.FindByNameAsync(principal.Identity.Name);

            if (user == null || user.RefreshToken != refreshToken)
            {
                throw new SecurityTokenException("Invalid refresh token");
            }

            user.RefreshToken = GenerateRefreshToken(user);
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                throw new Exception("Unable to create refresh token");
            }

            return new TokenResponseModel
            {
                AccessToken = await GenerateAccessToken(user),
                RefreshToken = user.RefreshToken
            };
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = _authSettings.SymmetricSecurityKey,
                    ValidateLifetime = false
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out _);

                return principal;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new ApiException(500, e.Message);
            }
        }
    }
}