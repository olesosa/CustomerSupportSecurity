using CS.Security.DTO;

namespace CS.Security.Interfaces;

public interface IAdminService
{
    Task<AdminDto> Create(AdminCreateDto adminDto);
    Task<bool> RemoveAdmin(Guid adminId);
    Task<List<AdminDto>> GetAll();
}