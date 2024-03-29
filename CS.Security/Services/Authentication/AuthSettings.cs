using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace CS.Security.Services.Authentication
{
    public class AuthSettings
    {
        public string SecretKey { get; set; }
        public double AccessTokenExpirationMinutes { get; set; }
        public double RefreshTokenExpirationMinutes { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public SecurityKey SymmetricSecurityKey { get =>
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey)); }
    }
}
