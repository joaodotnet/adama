using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ApplicationCore;
using ApplicationCore.Exceptions;
using ApplicationCore.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Infrastructure.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _appSettings;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IOptions<EmailSettings> appSettings, ILogger<EmailSender> logger)
        {
            _appSettings = appSettings.Value;
            _logger = logger;
        }
        public async Task SendEmailAsync(string fromEmail, string toEmail, string subject, string message, string bccEmails = null, IFormFile attachFile = null, List<(string, byte[])> files = null)
        {
            await Execute(fromEmail, toEmail, subject, message, bccEmails, attachFile, files);
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
                if (string.IsNullOrEmpty(ToEmail))
                    return;
                _logger.LogInformation("Sending Email...");
                var mail = new MimeMessage();
                mail.From.Add(new MailboxAddress(GetFromName(FromEmail), FromEmail));
                
                foreach (var item in ToEmail.Split(","))
                {
                    mail.To.Add(MailboxAddress.Parse(item));
                }
                if (!string.IsNullOrEmpty(bccEmails))
                {
                    foreach (var item in bccEmails.Split(","))
                    {
                        mail.Bcc.Add(MailboxAddress.Parse(item));
                    }
                }
                mail.Subject = subject;
                var builder = new BodyBuilder
                {
                    HtmlBody = message
                };


                if (attachFile != null)
                    builder.Attachments.Add(attachFile.FileName, attachFile.OpenReadStream());
                else if(files?.Count > 0)
                {
                    foreach (var (FileName, Bytes) in files)
                    {
                        if(Bytes != null)
                            builder.Attachments.Add(FileName, new MemoryStream(Bytes));
                    }
                }

                mail.Body = builder.ToMessageBody();
                mail.Priority = MessagePriority.Normal;

                using (SmtpClient smtp = new SmtpClient())
                {                    
                    smtp.Connect(_appSettings.SmtpServer, _appSettings.SmtpPort, _appSettings.SSL);
                    smtp.Authenticate(_appSettings.SmtpUsername, _appSettings.SmtpPassword);
                    await smtp.SendAsync(mail);
                    smtp.Disconnect(true);
                }
                _logger.LogInformation("Email send successuful!");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while sending email: {ex}");
                throw new SendEmailException(ex.ToString());
            }
        }

        private static string GetFromName(string fromEmail)
        {
            if (fromEmail.Contains("saborcomtradicao"))
                return "Sabor com Tradição";
            return "Dama no Jornal";
        }

        private static string CreateGenericBody(string textBody)
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
