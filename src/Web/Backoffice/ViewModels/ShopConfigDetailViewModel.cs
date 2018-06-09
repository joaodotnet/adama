using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Backoffice.ViewModels
{
    public class ShopConfigDetailViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Url Imagem")]
        public string PictureUri { get; set; }
        [Display(Name = "Titulo")]
        public string HeadingText { get; set; }
        [Display(Name = "Conteudo")]
        public string ContentText { get; set; }
        [Display(Name = "Link URL")]
        public string LinkButtonUri { get; set; }
        [Display(Name = "Texto Link")]
        public string LinkButtonText { get; set; }
        [Display(Name = "Ativo")]
        public bool IsActive { get; set; }
        [Display(Name = "Imagem")]
        public IFormFile Picture { get; set; }
        public int ShopConfigId { get; set; }
    }
}