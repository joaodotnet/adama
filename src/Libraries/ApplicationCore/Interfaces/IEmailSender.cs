using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{

    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message, string bccEmails = null, IFormFile attachFile = null);
        Task SendGenericEmailAsync(string email, string subject, string textBody, string bccEmails = null, List<(string, byte[])> files = null);
    }
}
