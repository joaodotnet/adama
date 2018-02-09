using Microsoft.AspNetCore.Http;
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
        [Display(Name = "Categorias")]
        public List<int> CategoriesId { get; set; } = new List<int>();
        [Display(Name = "Categorias")]
        public List<string> CategoriesName { get; set; } = new List<string>();
        public string PictureUri { get; set; }
        public IFormFile Picture { get; set; }
        
    }
}
