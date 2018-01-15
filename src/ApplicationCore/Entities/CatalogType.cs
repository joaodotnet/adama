namespace ApplicationCore.Entities
{
    public class CatalogType : BaseEntity
    {
        public string Code { get; set; } //To Remove       
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
