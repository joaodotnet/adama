using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Backoffice.ViewModels
{
    public class ProductTypeViewModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(25)]
        [Display(Name = "Código")]
        public string Code { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name = "Descrição")]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Categoria")]
        public int CategoryId { get; set; }
        public CategoryViewModel Category { get; set; }
    }
}
