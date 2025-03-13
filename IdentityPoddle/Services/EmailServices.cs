using IdentityPoddle.Entity;
using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Net.Mail;

namespace IdentityPoddle.Services
{
    public class EmailServices : IEmailSender<User>
    {
        private readonly IConfiguration _configuration;

        public EmailServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var fromEmail = _configuration["EmailSettings:FromEmail"];
            var fromPassword = _configuration["EmailSettings:AppPassword"];
            var smtpHost = _configuration["EmailSettings:SmtpHost"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);

            var fromAddress = new MailAddress(fromEmail, "Identity Poddle");
            var toAddress = new MailAddress(email);

            using var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };

            var smtp = new SmtpClient
            {
                Host = smtpHost,
                Port = smtpPort,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            smtp.Send(message);

            return Task.CompletedTask;
        }

        public Task SendConfirmationLinkAsync(User user, string email, string confirmationLink)
        {
            var subject = "Confirm your email";
            var body = $"Please confirm your account by clicking this link: {confirmationLink}";

            return SendEmailAsync(email, subject, body);
        }

        public void SendOtpEmail(string email, string otp)
        {
            var subject = "Your OTP Code";
            var body = $"Your OTP is: {otp}";

            SendEmailAsync(email, subject, body).Wait();
        }

        public Task SendPasswordResetCodeAsync(User user, string email, string resetCode)
        {
            var subject = "Password Reset Code";
            var body = $"Your password reset code is: {resetCode}";

            return SendEmailAsync(email, subject, body);
        }

        public Task SendPasswordResetLinkAsync(User user, string email, string resetLink)
        {
            var subject = "Password Reset Link";
            var body = $"Click the following link to reset your password: {resetLink}";

            return SendEmailAsync(email, subject, body);
        }
    }
}