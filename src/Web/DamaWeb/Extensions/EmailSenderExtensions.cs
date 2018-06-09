using ApplicationCore.Interfaces;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Mvc
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, "Dama no Jornal®: Confirme o seu email",
                $"Por favor confirme a sua conta <a href='{HtmlEncoder.Default.Encode(link)}'>clicando aqui</a>.");
        }

        public static Task SendResetPasswordAsync(this IEmailSender emailSender, string email, string callbackUrl)
        {
            return emailSender.SendEmailAsync(email, "Dama no Jornal®: Recuperar Password",
                $"Por favor recupere a password <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicando aqui</a>.");
        }
    }

}
