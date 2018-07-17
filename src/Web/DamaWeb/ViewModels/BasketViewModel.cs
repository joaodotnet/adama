using System;
using System.Collections.Generic;
using System.Linq;

namespace DamaWeb.ViewModels
{

    public class BasketViewModel
    {
        public int Id { get; set; }
        public List<BasketItemViewModel> Items { get; set; } = new List<BasketItemViewModel>();
        public string BuyerId { get; set; }
        public decimal DefaultShippingCost { get; set; } = 3.35m;

        public decimal SubTotal()
        {
            return Items.Sum(x => x.UnitPrice * x.Quantity);
        }

        public decimal Total()
        {
            return Math.Round(SubTotal() + DefaultShippingCost,2);
        }
    }
}
