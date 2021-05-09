
namespace ApplicationCore.Entities
{
    public class CatalogAttribute : BaseEntity
    {
        public AttributeType Type { get; set; }
        public string Name { get; set; }
        public int CatalogItemId { get; set; }
        public CatalogItem CatalogItem { get; set; }
        public int Stock { get; set; }
        //public int? ReferenceCatalogItemId { get; set; }
        //public CatalogItem ReferenceCatalogItem { get; set; }
    }
}