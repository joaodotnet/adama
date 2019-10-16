using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Exceptions
{
    public class SageException : Exception
    {
        public SageException(string statusCode, string message) : base($"Sage Response Status {statusCode} message: {message}")
        {

        }
    }
}
