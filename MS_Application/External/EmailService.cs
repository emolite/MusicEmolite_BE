using Microsoft.Extensions.Options;
using MS_Application.DataTransferObjects.Email;
using MS_Application.Services.Interfaces.External;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace MS_Application.External
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettingsDto _settings;

        public EmailService(IOptions<EmailSettingsDto> options)
        {
            _settings = options.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            using var message = new MailMessage
            {
                From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            message.To.Add(toEmail);

            using var client = new SmtpClient(_settings.Host, _settings.Port)
            {
                EnableSsl = _settings.EnableSsl,
                Credentials = new NetworkCredential(_settings.SenderEmail, _settings.AppPassword)
            };

            await client.SendMailAsync(message);
        }
    }
}
