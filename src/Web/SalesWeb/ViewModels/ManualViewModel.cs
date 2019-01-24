using System.ComponentModel.DataAnnotations;

namespace SalesWeb.ViewModels
{
    public class ManualViewModel
    {
        [Display(Name = "Descrição")]
        public string Description { get; set; }
        [Display(Name = "Quantidade")]
        public int Quantity { get; set; }
        [Display(Name = "Preço")]
        public decimal Price { get; set; }
    }
}
