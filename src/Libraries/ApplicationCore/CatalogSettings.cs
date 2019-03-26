namespace ApplicationCore
{
    public class CatalogSettings : AppSettings
    {
        public string CatalogBaseUrl { get; set; }
        public decimal DefaultShippingCost { get; set; } = 3.35m;
    }
}
