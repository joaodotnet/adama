
namespace ApplicationCore.Entities
{
    public class CatalogAttribute : BaseEntity
    {
        public string Sku { get; set; }
        public CatalogAttributeType Type { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal? Price { get; set; }
        public int CatalogItemId { get; set; }
        public CatalogItem CatalogItem { get; set; }
    }
}