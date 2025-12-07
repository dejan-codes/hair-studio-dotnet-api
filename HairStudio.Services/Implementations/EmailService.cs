using System.Net;
using System.Net.Mail;
using HairStudio.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace HairStudio.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var smtpClient = new SmtpClient(_configuration["Smtp:Host"])
            {
                Port = int.Parse(_configuration["Smtp:Port"]),
                Credentials = new NetworkCredential(_configuration["Smtp:Username"], _configuration["Smtp:Password"]),
                EnableSsl = true,
                UseDefaultCredentials = false
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["Smtp:From"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };
            mailMessage.To.Add(to);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
