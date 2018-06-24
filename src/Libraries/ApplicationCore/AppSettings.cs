using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore
{
    public class AppSettings
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public string FromEmail { get; set; }
        public string ToEmails { get; set; }
        public bool SSL { get; set; }
        public string AuthorizationURL { get; set; }
        public string SageApiBaseUrl { get; set; }
        public string AccessTokenURL { get; set; }
        public string ClientId { get; set; }
        public string CallbackURL { get; set; }
        public string ClientSecret { get; set; }
        public string SigningSecret { get; set; }
    }
}
