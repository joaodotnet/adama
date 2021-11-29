using System.ComponentModel.DataAnnotations;

namespace DamaAdmin.Shared.Models
{
    public class IllustrationTypeViewModel : BaseViewModel
    {
        [Required]
        [Display(Name = "Código")]
        [StringLength(25)]
        public string Code { get; set; }
    }
}
