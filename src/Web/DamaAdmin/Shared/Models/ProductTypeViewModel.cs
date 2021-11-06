using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ApplicationCore.DTOs;
using ApplicationCore.Entities;

namespace DamaAdmin.Shared.Models
{
    public class ProductTypeViewModel : BaseViewModel
    {       
        [Required]
        [StringLength(25)]
        [Display(Name = "Código")]
        public string Code { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Slug { get; set; }
        [Required]
        [Display(Name = "Categorias")]
        public List<string> CategoriesId { get; set; } = new List<string>();
        [Display(Name = "Categorias")]
        public List<string> CategoriesName { get; set; } = new List<string>();
        public string PictureUri { get; set; }
        [Required]
        [Display(Name = "Entrega (Min)")]
        public int DeliveryTimeMin { get; set; } = 2;
        [Required]
        [Display(Name = "Entrega (Max)")]
        public int DeliveryTimeMax { get; set; } = 3;
        [Required]
        [Display(Name = "Entrega (Unidade)")]
        public DeliveryTimeUnitType DeliveryTimeUnit { get; set; } = DeliveryTimeUnitType.Days;
        [Required]
        [Display(Name = "Preço")]
        public decimal Price { get; set; }
        [Display(Name = "Preço Adicional Frase/Nome")]
        public decimal? AdditionalTextPrice { get; set; }
        
        public IList<FileDetailViewModel> PictureTextHelpers { get; set; } = new List<FileDetailViewModel>();
        [Display(Name = "Peso (gramas)")]
        public int? Weight { get; set; }
        [Display(Name = "Meta Description")]
        [StringLength(160)]
        public string MetaDescription { get; set; }
        [Display(Name = "Title")]
        [StringLength(43)]
        public string Title { get; set; }

        [Display(Name = "Texto H1")]
        [StringLength(50)]
        public string H1Text { get; set; }
        [Display(Name = "Descrição")]
        public string Description { get; set; }
        [Display(Name = "Questão personalizar (loja)")]
        [StringLength(255)]
        public string Question { get; set; }
        public List<FileData> FormFileTextHelpers { get; set; } = new();
        public FileData Picture {get; set; }
    }
}
