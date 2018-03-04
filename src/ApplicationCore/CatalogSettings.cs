namespace ApplicationCore
{
    public class CatalogSettings
    {
        public string CatalogBaseUrl { get; set; }
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public string FromEmail { get; set; }
        public string ToEmails { get; set; }
        public bool SSL { get; set; }
    }
}
