using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Backoffice.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }                       
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
        public int CatalogIllustrationId { get; set; }
        public IllustrationViewModel CatalogIllustration { get; set; }
        [Required]
        [Display(Name = "Tipo de Produto")]
        public int CatalogTypeId { get; set; }
        public ProductTypeViewModel CatalogType { get; set; }
        [Display(Name = "SKU")]
        public string ProductSKU { get; set; }

        public IList<ProductAttributeViewModel> CatalogAttributes { get; set; } = new List<ProductAttributeViewModel>();  
    }
}
