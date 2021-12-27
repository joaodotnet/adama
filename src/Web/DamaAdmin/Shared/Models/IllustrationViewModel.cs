using System.ComponentModel.DataAnnotations;
using ApplicationCore.DTOs;

namespace DamaAdmin.Shared.Models
{
    public class IllustrationViewModel : BaseViewModel
    {
        [Display(Name = "Código")]
        [StringLength(25)]
        [Required]
        public string Code { get; set; }
        [Required]
        [Display(Name = "Tipo")]
        public int IllustrationTypeId { get; set; }
        public IllustrationTypeViewModel IllustrationType { get; set; }
    }
}
