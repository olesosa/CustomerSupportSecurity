namespace CS.Security.DTO;

public record UserInfoDto(
	Guid Id,
	string UserName,
	string Email,
	string RoleName);