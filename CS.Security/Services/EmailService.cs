using CS.Security.Interfaces;
using CS.Security.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using MimeKit;
using MimeKit.Text;

namespace CS.Security.Services
{
    public class EmailService : IEmailService
    {
      private static readonly string ApiIdentityAddress = Environments.ApiIdentityAddress;

      readonly UserManager<User> _userManager;

        public EmailService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        private async Task<string> CallBackUrl(User user, string code)
        {
            var callback_url = ApiIdentityAddress + "/api/Users/EmailVerification" + $"?userId={user.Id}&code={code}";

            return await Task.FromResult(callback_url);
        }

        public async Task<bool> SendEmail(User user)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callback_url = await CallBackUrl(user, code);

            try
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse("alexbobr1337@gmail.com"));
                email.To.Add(MailboxAddress.Parse(user.Email));
                email.Subject = "Email verification";
                email.Body = email.Body = new TextPart(TextFormat.Html)
                {
                    Text = $@"<!DOCTYPE html>
<html>
<head>
  <meta http-equiv=""Content-Type"" content=""text/html"" charset=""UTF-8"" />
  <title>Email Verification</title>
  <link href=""https://fonts.googleapis.com/css2?family=Lato&display=swap"" rel=""stylesheet"">
  <style>
    .button-container {{
      text-align: center;
    }}
    .verify-button {{
      background-color: transparent; /* Transparent button background */
      color: black; /* Text color */
      width: 175px;
      height: 35px;
      font-family: 'Lato', sans-serif;
      font-size: 1em;
      border: 2px solid black; /* Black border */
      border-radius: 20px;
      text-decoration: none;
      display: inline-block;
      line-height: 35px;
      text-align: center;
    }}
    .email-text {{
      color: #333;
      font-size: 1em;
    }}
  </style>
</head>
<body>

  <table cellpadding=""0"" cellspacing=""0"" border=""0"">
    <tr>
      <td>
        <h2>Email Verification</h2>
        <p class=""email-text"">Thank you for signing up! To verify your email address, please click the link below:</p>
      </td>
    </tr>
    <tr>
      <td style=""text-align: center;"">
        <div class=""button-container"">
          <a class=""verify-button"" href=""{callback_url}"">Verify Email</a>
        </div>
      </td>
    </tr>
    <tr>
      <td>
        <p class=""email-text"">If you didn't sign up for this service, please ignore this email.</p>
        <p class=""email-text"" style=""margin-bottom: 5px;"">Best regards,</p>
        <p class=""email-text"" style=""margin: 0;"">Little Kittens Pizzeria</p>
      </td>
    </tr>
  </table>
</body>
</html>"
                };

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync("alexbobr1337@gmail.com", "hklu emus pdgn dpxk");

                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}