using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ApplicationCore.DTOs;
using ApplicationCore.Entities;

namespace DamaAdmin.Shared.Models
{
    public class ProductViewModel : BaseViewModel
    {
        [Required]
        [StringLength(100)]
        public string Slug { get; set; }
        [Display(Name = "Descrição")]
        public string Description { get; set; }
        [Display(Name = "Preço")]
        public decimal? Price { get; set; }
        public decimal? ProductTypePrice { get; set; }
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
        public bool IsFeatured { get; set; }
        [Display(Name = "Personalizar")]
        public bool CanCustomize { get; set; }
        [Display(Name = "URL da Imagem Principal")]
        public string PictureUri { get; set; }

        [Display(Name = "Stock")]
        public int Stock { get; set; }
        [Display(Name = "Meta Description")]
        [StringLength(160)]
        public string MetaDescription { get; set; }
        [Display(Name = "Title")]
        [StringLength(43)]
        public string Title { get; set; }
        [Display(Name = "Desconto")]
        public decimal? Discount { get; set; }
        [Display(Name = "Indisponível")]
        public bool IsUnavailable { get; set; }

        [Display(Name = "Categorias")]
        public List<string> CategoriesName { get; set; } = new List<string>();

        [NotMapped]
        public List<FileData> PicturesToUpload { get; set; } = new();
        //public List<FileDetailViewModel> OtherPictures { get; set; } = new();
        [NotMapped]
        public List<ProductPictureViewModel> Pictures { get; set; } = new();
        [NotMapped]
        public List<CatalogCategoryViewModel> Categories { get; set; } = new();
        public List<CatalogReferenceViewModel> References { get; set; } = new();

        public string DisplayCatalogTypeName
        {
            get
            {
                return $"{this.CatalogType?.Code} - {this.CatalogType?.Name}";
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
