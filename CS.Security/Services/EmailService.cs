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

        public async Task<bool> SendEmail(User user)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var url = ApiIdentityAddress + "/Users/EmailVerification" + $"?userId={user.Id}&code={code}";

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("alexbobr1337@gmail.com"));
            email.To.Add(MailboxAddress.Parse(user.Email));
            email.Subject = "Email verification";
            email.Body = email.Body = new TextPart(TextFormat.Html)
            {
                Text = $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Email Verification</title>
    <style>
        /* Reset styles */
        body, h1, p {{
            margin: 0;
            padding: 0;
        }}

        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            color: #333;
        }}

        .container {{
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            background-color: #fff;
            border-radius: 8px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        }}

        h1 {{
            font-size: 24px;
            margin-bottom: 20px;
        }}

        p {{
            font-size: 16px;
            line-height: 1.5;
            margin-bottom: 20px;
        }}

        .btn {{
            display: inline-block;
            padding: 10px 20px;
            background-color: #007bff;
            color: #fff;
            text-decoration: none;
            border-radius: 5px;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <h1>Email Verification</h1>
        <p>Dear User,</p>
        <p>Thank you for signing up for our service. To verify your email address and activate your account, please click the button below:</p>
        <a href=""{url}"" class=""btn"">Verify Email</a>
        <p>If you did not sign up for our service, please disregard this email.</p>
        <p>For any questions or assistance, please contact our Customer Support at <a href=""mailto:customersupport@example.com"">customersupport@example.com</a>.</p>
        <p>Best Regards,<br>Your Service Team</p>
    </div>
</body>
</html>
"
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync("alexbobr1337@gmail.com", "hklu emus pdgn dpxk");

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);

            return true;
        }
    }
}