using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{

    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message, string bccEmails = null, IFormFile attachFile = null);
    }
}
