using System;
using System.Collections.Generic;
using System.Text;
using ApplicationCore.Interfaces;

namespace ApplicationCore.Entities
{
    public class ShippingPriceWeight : BaseEntity, IAggregateRoot
    {
        public int MinWeight { get; set; }
        public int? MaxWeight { get; set; }
        public decimal Price { get; set; }
        public ShippingPriceCountryType Country { get; set; } = ShippingPriceCountryType.Portugal;
    }

    public enum ShippingPriceCountryType
    {
        Portugal
    }
}
