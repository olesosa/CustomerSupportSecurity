using System.ComponentModel.DataAnnotations;

namespace CS.Security.DTO
{
    public class TokenDto
    {
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
    }
}
