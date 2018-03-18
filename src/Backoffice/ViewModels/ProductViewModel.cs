using Microsoft.AspNetCore.Http;
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
        public string Sku { get; set; }
        [Display(Name = "Loja")]
        public bool ShowOnShop { get; set; }
        [Display(Name = "Novidade")]
        public bool IsNew { get; set; }
        [Display(Name = "Destaque")]
        public bool IsFeatured{ get; set; }
        [Display(Name = "Imagem Principal")]
        public IFormFile Picture { get; set; }
        [Display(Name = "URL da Imagem Principal")]
        public string PictureUri { get; set; }
        [Display(Name = "Imagens do Produto")]
        public List<IFormFile> OtherPictures { get; set; }

        public IList<ProductAttributeViewModel> CatalogAttributes { get; set; } = new List<ProductAttributeViewModel>();
        [Display(Name = "Imagens do Produto")]
        public IList<ProductPictureViewModel> CatalogPictures { get; set; } = new List<ProductPictureViewModel>();
        [Display(Name = "Categorias")]
        public IList<CatalogCategoryViewModel> CatalogCategories { get; set; } = new List<CatalogCategoryViewModel>();

        public string DisplayCatalogTypeName {
            get {
                return $"{this.CatalogType?.Code} - {this.CatalogType?.Description}";
            }
        }
        public string DisplayIllustrationName
        {
            get
            {
                return $"{this.CatalogIllustration?.Code} - {this.CatalogIllustration?.Name}";
            }
        }
    }
}
