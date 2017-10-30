using ApplicationCore.Entities;

namespace Backoffice.RazorPages.ViewModels
{
    public class ProductAttributeViewModel
    {
        public int Id { get; set; }
        public ProductAttributeType Type { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal? Price { get; set; }
        public int ProductId { get; set; }
    }
}