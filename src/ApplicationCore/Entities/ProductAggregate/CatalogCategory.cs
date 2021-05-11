namespace ApplicationCore.Entities
{
    public class CatalogCategory: BaseEntity
    {
        public int CatalogItemId { get; private set; }
        public CatalogItem CatalogItem { get; private set; }
        public int CategoryId { get; private set; }
        public Category Category { get; private set; }
        public CatalogCategory(int categoryId)
        {
            CategoryId = categoryId;
        }
    }
}