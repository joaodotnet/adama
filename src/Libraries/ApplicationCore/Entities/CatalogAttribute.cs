
namespace ApplicationCore.Entities
{
    public class CatalogAttribute : BaseEntity
    {
        public int AttributeId { get; set; }
        public Attribute Attribute { get; set; }
        public decimal? Price { get; set; }
        public int CatalogItemId { get; set; }
        public CatalogItem CatalogItem { get; set; }
        public int? ReferenceCatalogItemId { get; set; }
        public CatalogItem ReferenceCatalogItem { get; set; }
    }
}