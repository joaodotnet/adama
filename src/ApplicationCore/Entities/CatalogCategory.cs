namespace ApplicationCore.Entities
{
    public class CatalogCategory: BaseEntity
    {
        public int CatalogItemId { get; set; }
        public CatalogItem CatalogItem { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}