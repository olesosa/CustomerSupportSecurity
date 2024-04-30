namespace CS.Security.DTO;

public record UserDto(
	Guid Id,
	string UserName,
	string Email);