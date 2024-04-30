namespace CS.Security.Services.Authentication;

public class TokenResponseModel
{
    public string AccessToken { get; set; } = default!;
    
    public string RefreshToken { get; set; } = default!;
}