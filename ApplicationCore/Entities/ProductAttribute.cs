
namespace ApplicationCore.Entities
{
    public class ProductAttribute : BaseEntity
    {
        public ProductAttributeType Type { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal? Price { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}