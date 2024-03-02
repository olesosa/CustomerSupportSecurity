using System.ComponentModel.DataAnnotations;

namespace CS.Security.DTO
{
    public class TokenRequestDto
    {
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
    }
}
