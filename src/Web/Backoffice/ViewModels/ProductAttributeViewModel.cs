using ApplicationCore.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Backoffice.ViewModels
{
    public class ProductAttributeViewModel
    {
        public int Id { get; set; }
        public AttributeType Type { get; set; }
        [Display(Name = "Nome")]
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        public int CatalogItemId { get; set; }
        public ProductViewModel CatalogItem { get; set; }        
        public bool ToRemove { get; set; }
        public int Stock { get; set; }
    }
}