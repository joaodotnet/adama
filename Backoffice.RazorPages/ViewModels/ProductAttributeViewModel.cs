using ApplicationCore.Entities;
using System.ComponentModel.DataAnnotations;

namespace Backoffice.RazorPages.ViewModels
{
    public class ProductAttributeViewModel
    {
        public int Id { get; set; }        
        [Display(Name = "Tipo")]
        public ProductAttributeType Type { get; set; }
        [StringLength(25)]
        [Display(Name = "Código")]
        public string Code { get; set; }
        [StringLength(100)]
        [Display(Name = "Nome")]
        public string Name { get; set; }
        [Display(Name = "Preço")]
        public decimal? Price { get; set; }
        public int ProductId { get; set; }
        public bool ToRemove { get; set; }
        [Display(Name = "SKU")]
        public string ProductSKU { get; set; }
    }
}