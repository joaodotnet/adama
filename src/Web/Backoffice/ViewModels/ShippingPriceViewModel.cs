using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Backoffice.ViewModels
{
    public class ShippingPriceViewModel
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Peso Minimo")]
        public int MinWeight { get; set; }
        [Display(Name = "Peso Máximo")]
        public int? MaxWeight { get; set; }
        [Display(Name = "Preço")]
        public decimal Price { get; set; }
    }
}
