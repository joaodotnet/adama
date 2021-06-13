using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ApplicationCore;
using ApplicationCore.DTOs;

namespace DamaWeb.ViewModels
{

    public class BasketViewModel
    {
        public int Id { get; set; }
        public List<BasketItemViewModel> Items { get; set; } = new List<BasketItemViewModel>();
        public string BuyerId { get; set; }
        public decimal DefaultShippingCost { get; set; }
        [MaxLength(5000)]
        public string Observations { get; set; }
        public string CouponText { get; set; }
        public decimal? Discount { get; set; }
        public bool CanSubmit { get; set; } = false;

        public bool HasCustomizeItems
        {
            get
            {
                return Items.Any(x => x.IsFromCustomize);
            }
        }
        public DeliveryTimeDTO DeliveryTime { get; set; } = new DeliveryTimeDTO();
        public bool ShowShippingCost { get; set; }
      

        public decimal SubTotal()
        {
            return Items.Sum(x => x.UnitPrice * x.Quantity) - (Discount ?? 0);
        }

        public decimal Total()
        {
            var shippingCost = ShowShippingCost ? DefaultShippingCost : 0;
            var total = Math.Round(SubTotal() + shippingCost,2);
            return total;
        }

        public decimal TotalWithShipping()
        {
            return Math.Round(SubTotal() + DefaultShippingCost, 2);
        }

        public decimal TotalWithoutShipping()
        {
            return Math.Round(SubTotal(), 2);
            
        }
    }
}
