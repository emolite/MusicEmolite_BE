using System.Threading.Tasks;

namespace MS_Application.Services.Interfaces.External
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlBody);
    }
}
