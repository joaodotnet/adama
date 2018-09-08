using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{

    public interface IEmailSender
    {
        Task SendEmailAsync(string fromEmail, string toEmail, string subject, string message, string bccEmails = null, IFormFile attachFile = null, List<(string, byte[])> files = null);
        Task SendGenericEmailAsync(string fromEmail, string toEmail, string subject, string textBody, string bccEmails = null, List<(string, byte[])> files = null);
    }
}
