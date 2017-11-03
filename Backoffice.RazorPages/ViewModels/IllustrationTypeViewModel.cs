using System.ComponentModel.DataAnnotations;

namespace Backoffice.RazorPages.ViewModels
{
    public class IllustrationTypeViewModel
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Código")]
        [StringLength(25)]
        public string Code { get; set; }
        [Display(Name = "Name")]
        [StringLength(100)]
        public string Name { get; set; }
    }
}