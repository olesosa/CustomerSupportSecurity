using CS.Security.Models;

namespace CS.Security.Interfaces;

public interface IEmailService
{
    Task<bool> SendEmail(User user);
}