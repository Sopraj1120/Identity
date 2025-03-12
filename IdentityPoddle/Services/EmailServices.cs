using System.Net;
using System.Net.Mail;
using static System.Net.WebRequestMethods;

namespace IdentityPoddle.Services
{
    public class EmailServices
    {
        private readonly IConfiguration _configuration;

        public EmailServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void SendOtpEmail(string email, string otp)
        {
            var fromEmail = _configuration["EmailSettings:FromEmail"];
            var fromPassword = _configuration["EmailSettings:AppPassword"]; // இங்கே App Password பயன்படும்
            var smtpHost = _configuration["EmailSettings:SmtpHost"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);

            var fromAddress = new MailAddress(fromEmail, "Identity Poddle");
            var toAddress = new MailAddress(email);
          
            const string subject = "Your OTP Code";
            string body = $"Your OTP is: {otp}";

            var smtp = new SmtpClient
            {
                Host = smtpHost,
                Port = smtpPort,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            };
            smtp.Send(message);
        }
    }
}
