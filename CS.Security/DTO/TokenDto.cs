namespace CS.Security.DTO;

public record TokenDto(
	string Token,
	string RefreshToken);