using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore
{
    public class AppSettings
    {
        //public string SmtpServer { get; set; }
        //public int SmtpPort { get; set; }
        //public string SmtpUsername { get; set; }
        //public string SmtpPassword { get; set; }
        //public string FromOrderEmail { get; set; }
        //public string FromInfoEmail { get; set; }
        //public string ToEmails { get; set; }
        //public bool SSL { get; set; }
        public SageSettings Sage { get; set; }
        public string InvoiceNameFormat { get; set; } = "DamanoJornalFatura#{0}.pdf";
        public string ReceiptNameFormat { get; set; } = "DamanoJornalRecibo#{0}.pdf";
        public string MailChimpListId { get; set; }
        public string MailChimpBasicAuth { get; set; }
        public string MailChimpBaseUrl { get; set; }
    }


    public class SageSettings
    {
        public string AuthorizationURL { get; set; }
        public string SageApiBaseUrl { get; set; }
        public string AccessTokenURL { get; set; }
        public string ClientId { get; set; }
        public string CallbackURL { get; set; }
        public string ClientSecret { get; set; }
        public string SigningSecret { get; set; }
    }

    public class EmailSettings
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public string FromOrderEmail { get; set; }
        public string FromInfoEmail { get; set; }
        public string CCEmails { get; set; }
        public bool SSL { get; set; }
    }
}
