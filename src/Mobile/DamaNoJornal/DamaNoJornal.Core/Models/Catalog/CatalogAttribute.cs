namespace DamaNoJornal.Core.Models.Catalog
{
    public class CatalogAttribute
    {
        public int Id { get; set; }
        public string Sku { get; set; }
        public AttributeType Type { get; set; }
        public string Name { get; set; }
        public decimal? Price { get; set; }                
    }
}