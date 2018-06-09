using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Backoffice.ViewModels
{
    public class ProductPictureViewModel
    {
        public int Id { get; set; }
        public string PictureUri { get; set; }
        [Display(Name = "Ativo")]
        public bool IsActive { get; set; }
        [Display(Name = "Ordem")]
        public int Order { get; set; }
        public int CatalogItemId { get; set; }
        public IFormFile Picture { get; set; }
        public bool ToRemove { get; set; }
    }
}