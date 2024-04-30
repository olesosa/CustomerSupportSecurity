using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace CS.Security.Services.Authentication;

public class AuthSettings
{
	public string SecretKey { get; set; } = string.Empty;
	public double AccessTokenExpirationMinutes { get; set; }
	public double RefreshTokenExpirationMinutes { get; set; }
	public string Issuer { get; set; } = default!;
	public string Audience { get; set; } = default!;
	public SecurityKey SymmetricSecurityKey => new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));
}