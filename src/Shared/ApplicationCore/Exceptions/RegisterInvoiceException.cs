using System;

namespace ApplicationCore.Exceptions
{
    public class RegisterInvoiceException : Exception
    {
        public RegisterInvoiceException() : base($"Error register invoice...")
        {
        }

        
        public RegisterInvoiceException(string message) : base($"Error register invoice: {message}")
        {
        }

        public RegisterInvoiceException(string message, Exception innerException) : base($"Error register invoice: {message}", innerException)
        {
        }
    }
}
