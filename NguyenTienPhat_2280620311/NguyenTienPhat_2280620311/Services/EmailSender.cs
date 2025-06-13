using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace NguyenTienPhat_2280620311.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;
        public EmailSender(IConfiguration config)
        {
            _config = config;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var settings = _config.GetSection("EmailSettings");
            var smtpClient = new SmtpClient(settings["SmtpServer"], int.Parse(settings["SmtpPort"]))
            {
                Credentials = new NetworkCredential(settings["SmtpUser"], settings["SmtpPass"]),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(settings["SmtpUser"]),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);

            return smtpClient.SendMailAsync(mailMessage);
        }
    }
} 