using System;

namespace ApplicationCore.Exceptions
{
    public class SendEmailException : Exception
    {
        public SendEmailException() : base($"Error sending email...")
        {
        }

        
        public SendEmailException(string message) : base($"Error sending email: {message}")
        {
        }

        public SendEmailException(string message, Exception innerException) : base($"Error sending email: {message}", innerException)
        {
        }
    }
}
