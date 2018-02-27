using System;
using System.Collections.Generic;
using System.Linq;

namespace Web.ViewModels
{

    public class BasketViewModel
    {
        public int Id { get; set; }
        public List<BasketItemViewModel> Items { get; set; } = new List<BasketItemViewModel>();
        public string BuyerId { get; set; }
        public decimal DefaultShippingCost { get; set; } = 3.85m;

        public decimal SubTotal()
        {
            return Math.Round(Items.Sum(x => x.UnitPrice * x.Quantity), 2);
        }

        public decimal Total()
        {
            return SubTotal() + DefaultShippingCost;
        }
    }
}
