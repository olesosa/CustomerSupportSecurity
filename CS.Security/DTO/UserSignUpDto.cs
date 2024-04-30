namespace CS.Security.DTO;

public record UserSignUpDto(
	string UserName,
	string Email,
	string Password);