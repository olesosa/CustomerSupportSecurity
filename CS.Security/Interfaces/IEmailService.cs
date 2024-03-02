using CS.Security.Models;

namespace CS.Security.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmail(User user);
        Task<string> CallBackUrl(User user, string code);
        Task<bool> VerifyEmail(User user, string code);
    }
}
