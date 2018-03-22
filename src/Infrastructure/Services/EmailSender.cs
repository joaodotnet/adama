using ApplicationCore;
using ApplicationCore.Interfaces;
using System;
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
        public Task SendEmailAsync(string email, string subject, string message, string bccEmails = null)
        {
            // TODO: Wire this up to actual email sending logic via SendGrid, local SMTP, etc.
            Execute(email, subject, message, bccEmails).Wait();

            return Task.FromResult(0);
        }
        public async Task Execute(string email, string subject, string message, string bccEmails)
        {
            try
            {
                MailMessage mail = new MailMessage()
                {
                    From = new MailAddress(_appSettings.FromEmail, "Dama no Jornal")
                };
                mail.To.Add(new MailAddress(email));
                if(!string.IsNullOrEmpty(bccEmails))
                    mail.Bcc.Add(bccEmails);
                mail.Subject = subject;
                mail.Body = message;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.Normal;

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
    }
}
