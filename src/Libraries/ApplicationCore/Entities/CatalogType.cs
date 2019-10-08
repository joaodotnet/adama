using ApplicationCore.DTOs;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Entities
{
    public class CatalogType : BaseEntity
    {
        public string Code { get; set; } //To Remove       
        public string Name { get; set; }
        public string PictureUri { get; set; }
        public int DeliveryTimeMin { get; set; } = 2;
        public int DeliveryTimeMax { get; set; } = 3;
        public DeliveryTimeUnitType DeliveryTimeUnit { get; set; } = DeliveryTimeUnitType.Days;
        public decimal Price { get; set; }
        public decimal? AdditionalTextPrice { get; set; }
        public int? Weight { get; set; }
        public string MetaDescription { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }

        public string H1Text { get; set; }
        public string Description { get; set; }
        public string Question { get; set; }

        public ICollection<CatalogTypeCategory> Categories { get; set; }
        public ICollection<CatalogItem> CatalogItems { get; set; }
        public ICollection<FileDetail> PictureTextHelpers { get; set; }
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
