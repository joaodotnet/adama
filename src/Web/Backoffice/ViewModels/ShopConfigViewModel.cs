using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Backoffice.ViewModels
{
    public class ShopConfigViewModel
    {
        public int Id { get; set; }
        public ShopConfigType Type { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        [Display(Name = "Ativo")]
        public bool IsActive { get; set; }       

        public List<ShopConfigDetailViewModel> Details { get; set; } = new List<ShopConfigDetailViewModel>();
    }
}
