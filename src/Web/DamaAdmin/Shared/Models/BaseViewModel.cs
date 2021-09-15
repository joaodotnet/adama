using System.ComponentModel.DataAnnotations;

namespace DamaAdmin.Shared.Models
{
    public abstract class BaseViewModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name = "Nome")]
        public string Name { get; set; }
    }
}
