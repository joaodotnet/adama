using ApplicationCore.DTOs;
using ApplicationCore.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Entities
{
    public class CatalogType : BaseEntity, IAggregateRoot
    {
        public string Code { get; private set; } //To Remove       
        public string Name { get; private set; }
        public string PictureUri { get; private set; }
        public int DeliveryTimeMin { get; private set; } = 2;
        public int DeliveryTimeMax { get; private set; } = 3;
        public DeliveryTimeUnitType DeliveryTimeUnit { get; private set; } = DeliveryTimeUnitType.Days;
        public decimal Price { get; private set; }
        public decimal? AdditionalTextPrice { get; private set; }
        public int? Weight { get; private set; }
        public string MetaDescription { get; private set; }
        public string Title { get; private set; }
        public string Slug { get; private set; }

        public string H1Text { get; private set; }
        public string Description { get; private set; }
        public string Question { get; private set; }

        private readonly List<CatalogTypeCategory> _categories = new();
        public IReadOnlyCollection<CatalogTypeCategory> Categories => _categories.AsReadOnly();
        private readonly List<CatalogItem> _catalogItems = new();
        public IReadOnlyCollection<CatalogItem> CatalogItems => _catalogItems.AsReadOnly();
        private readonly List<FileDetail> _pictureTextHelpers = new();
        public ICollection<FileDetail> PictureTextHelpers => _pictureTextHelpers.AsReadOnly();

        public CatalogType(string name, string pictureUri)
        {
            Name = name;
            Code = name;
            PictureUri = pictureUri;
        }

        public void UpdatePicture(string pictureUri)
        {
            PictureUri = pictureUri;
        }

        public void AddCategory(CatalogTypeCategory catalogTypeCategory)
        {
            _categories.Add(catalogTypeCategory);
        }

        public void AddPictureTextHelper(FileDetail pictureTextHelper)
        {
            _pictureTextHelpers.Add(pictureTextHelper);
        }

        public void Update(string code, string name, int deliveryTimeMin, int deliveryTimeMax, DeliveryTimeUnitType deliveryTimeUnit, decimal price, decimal? additionalTextPrice, int? weight, string metaDescription, string title, string slug)
        {
            Code = code;
            Name = name;
            DeliveryTimeMin = deliveryTimeMin;
            DeliveryTimeMax = deliveryTimeMax;
            DeliveryTimeUnit = deliveryTimeUnit;
            Price = price;
            AdditionalTextPrice = additionalTextPrice;
            Weight = weight;
            MetaDescription = metaDescription;
            Title = title;
            Slug = slug;
        }
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
