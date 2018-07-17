using ApplicationCore.DTOs;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Entities
{
    public class CatalogType : BaseEntity
    {
        public string Code { get; set; } //To Remove       
        public string Description { get; set; }
        public string PictureUri { get; set; }
        public int DeliveryTimeMin { get; set; } = 2;
        public int DeliveryTimeMax { get; set; } = 3;
        public DeliveryTimeUnitType DeliveryTimeUnit { get; set; } = DeliveryTimeUnitType.Days;
        public decimal Price { get; set; }
        public decimal? AdditionalTextPrice { get; set; }
        public decimal ShippingCost { get; set; }
        //public int CategoryId { get; set; }
        //public Category Category { get; set; }

        public ICollection<CatalogTypeCategory> Categories { get; set; }
        public ICollection<CatalogItem> CatalogItems { get; set; }
    }

    public enum DeliveryTimeUnitType
    {
        [Display(Name = "dias")]
        Days,
        [Display(Name = "semanas")]
        Weeks,
        [Display(Name = "meses")]
        Months
    }
}
