using Microsoft.AspNetCore.Identity;

namespace CS.Security.Models
{
    public class User : IdentityUser<Guid>
    {
        public string? RefreshToken { get; set; }
    }
}
