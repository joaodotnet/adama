using ApplicationCore.Interfaces;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Mvc
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string fromEmail, string toEmail, string link)
        {
            return emailSender.SendGenericEmailAsync(fromEmail, toEmail, "Dama no Jornal® - Confirme o seu email",
                $"Por favor confirme a sua conta <a href='{HtmlEncoder.Default.Encode(link)}'>clicando aqui</a>.");
        }

        public static Task SendResetPasswordAsync(this IEmailSender emailSender, string fromEmail, string toEmail, string callbackUrl)
        {
            return emailSender.SendGenericEmailAsync(fromEmail, toEmail, "Dama no Jornal® - Recuperar Password",
                $"Por favor recupere a password <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicando aqui</a>.");
        }


    }

}
