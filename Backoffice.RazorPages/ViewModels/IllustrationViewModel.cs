using ApplicationCore.Entities;
using System.ComponentModel.DataAnnotations;

namespace Backoffice.RazorPages.ViewModels
{
    public class IllustrationViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Código")]
        [StringLength(25)]
        [Required]
        public string Code { get; set; }
        [Display(Name = "Nome")]
        [StringLength(100)]
        [Required]
        public string Name { get; set; }
        [Display(Name = "Url")]
        [StringLength(255)]
        public string PictureUri { get; set; }
        [Required]
        [Display(Name = "Tipo")]
        public int IllustrationTypeId { get; set; }
        public IllustrationTypeViewModel IllustrationType { get; set; }
    }
}