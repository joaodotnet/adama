using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Backoffice.RazorPages.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }        
        [Required]
        [StringLength(25)]
        [Display(Name = "Código")]
        public string Code { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name = "Nome")]
        public string Name { get; set; }
        [StringLength(255)]
        [Display(Name = "Descrição")]
        public string Description { get; set; }                
        [Required]
        [Display(Name = "Preço")]
        public decimal Price { get; set; }
        [Display(Name = "Ilustração")]
        [Required]
        public int IllustrationId { get; set; }
        public IllustrationViewModel Illustation { get; set; }
        [Required]
        [Display(Name = "Tipo de Produto")]
        public int ProductTypeId { get; set; }
        public ProductTypeViewModel ProductType { get; set; }
        [Display(Name = "SKU")]
        public string ProductSKU { get; set; }

        public IList<ProductAttributeViewModel> ProductAttributes { get; set; } = new List<ProductAttributeViewModel>();  
    }
}
