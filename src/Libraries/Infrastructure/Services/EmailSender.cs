using ApplicationCore;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        private readonly AppSettings _appSettings;
        //private readonly IAppLogger<EmailSender> _logger;

        public EmailSender(AppSettings appSettings/*, IAppLogger<EmailSender> logger*/)
        {
            _appSettings = appSettings;
            //_logger = logger;
        }
        public async Task SendEmailAsync(string fromEmail, string toEmail, string subject, string message, string bccEmails = null, IFormFile attachFile = null)
        {
            // TODO: Wire this up to actual email sending logic via SendGrid, local SMTP, etc.
            await Execute(fromEmail, toEmail, subject, message, bccEmails, attachFile);
            //await SendGenericEmailAsync(fromEmail, toEmail, subject, message, bccEmails);
        }

        public async Task SendGenericEmailAsync(string fromEmail, string toEmail, string subject, string textBody, string bccEmails = null, List<(string,byte[])> files = null)
        {
            var message = CreateGenericBody(textBody);
            await Execute(fromEmail, toEmail, subject, message, bccEmails, null,files);
        }
        public async Task Execute(string FromEmail, string ToEmail, string subject, string message, string bccEmails, IFormFile attachFile, List<(string FileName, byte[] Bytes)> files = null)
        {
            try
            {
                MailMessage mail = new MailMessage()
                {
                    From = new MailAddress(FromEmail, "Dama no Jornal®")
                };
                mail.To.Add(ToEmail);
                if (!string.IsNullOrEmpty(bccEmails))
                    mail.Bcc.Add(bccEmails);
                mail.Subject = subject;
                mail.Body = message;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.Normal;
                if (attachFile != null)
                    mail.Attachments.Add(new Attachment(attachFile.OpenReadStream(), attachFile.FileName));
                else if(files != null)
                {
                    foreach (var item in files)
                    {
                        mail.Attachments.Add(new Attachment(new MemoryStream(item.Bytes), item.FileName));
                    }
                }

                using (SmtpClient smtp = new SmtpClient(_appSettings.SmtpServer, _appSettings.SmtpPort))
                {
                    smtp.Credentials = new NetworkCredential(_appSettings.SmtpUsername, _appSettings.SmtpPassword);
                    smtp.EnableSsl = _appSettings.SSL;
                    await smtp.SendMailAsync(mail);
                }
            }
            catch (Exception ex)
            {
                //_logger.LogWarning($"Error while sending email: {ex.Message}");
                throw ex;
            }
        }

        private string CreateGenericBody(string textBody)
        {
            string body = $@"
<table style='width:550px;'>
    <tr>
        <td width='400px' style='vertical-align:bottom'>
            {textBody}
        </td>
        <td>
            <img src='https://www.damanojornal.com/loja/images/dama_bird.png' width='150' />
        </td>
    </tr>
</table>
<div style='width:550px'>
    <img width='100%' src='https://www.damanojornal.com/loja/images/linha-coracao.png' />
</div>    
<div style='margin-top:20px;text-align:center;width:550px'>
    <strong>Muito Obrigada,</strong>
</div>
<div style='color: #EF7C8D;text-align:center;width:550px'>
    ❤ Dama no Jornal®
</div>
<div style='text-align:center;width:550px'>
    <img width='100' src='https://www.damanojornal.com/loja/images/logo_name.png' />
</div>";
            return body;
        }
    }
}
