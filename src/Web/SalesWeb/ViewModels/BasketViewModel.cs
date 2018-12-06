using System;
using System.Collections.Generic;
using System.Linq;

namespace SalesWeb.ViewModels
{

    public class BasketViewModel
    {
        public int Id { get; set; }
        public List<BasketItemViewModel> Items { get; set; } = new List<BasketItemViewModel>();
        public string BuyerId { get; set; }
        public decimal DefaultShippingCost { get; set; }

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
