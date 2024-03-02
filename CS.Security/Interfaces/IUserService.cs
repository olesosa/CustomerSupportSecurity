using CS.Security.DTO;
using CS.Security.Models;
using CS.Security.Servises.Authentication;

namespace CS.Security.Interfaces
{
    public interface IUserService
    {
        Task<bool> Create(UserSignUpDto user);
        Task<User?> GetById(Guid userId);
        Task<User?> GetByEmail(string email);
        Task<TokenResponseModel?> VerifyToken(TokenRequestDto tokenRequest);
    }
}
