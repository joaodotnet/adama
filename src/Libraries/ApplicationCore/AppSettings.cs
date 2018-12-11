using System;
using System.Collections.Generic;
using System.Text;
using ApplicationCore.DTOs;
using ApplicationCore.Entities;

namespace ApplicationCore
{
    public class AppSettings
    {
        public SageSettings Sage { get; set; }
        public EmailSettings Email { get; set; }
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
        public List<SageBankingSettings> SageBankings { get; set; }
    }

    public class SageBankingSettings
    {
        public PaymentType Type { get; set; }
        public string SageTypeId { get; set; }
        public string BankId { get; set; }
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
