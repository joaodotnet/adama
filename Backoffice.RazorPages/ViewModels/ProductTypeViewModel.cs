using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Backoffice.RazorPages.ViewModels
{
    public class ProductTypeViewModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(25)]
        public string Code { get; set; }
        [Required]
        [StringLength(100)]
        public string Description { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public CategoryViewModel Category { get; set; }
    }
}
