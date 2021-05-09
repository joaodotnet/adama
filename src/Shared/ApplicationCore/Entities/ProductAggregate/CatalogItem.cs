using System;
using System.Collections.Generic;

namespace ApplicationCore.Entities
{
    public class CatalogItem : BaseEntity
    {
        public string Sku { get; private set; }
        public string Name { get; private set; }
        public string Slug { get; private set; }
        public string Description { get; private set; }
        public decimal? Price { get; private set; }
        public string PictureUri { get; private set; }
        public int CatalogTypeId { get; private set; }
        public CatalogType CatalogType { get; private set; }
        public int CatalogIllustrationId { get; private set; }
        public CatalogIllustration CatalogIllustration { get; set; }
        public bool ShowOnShop { get; private set; }
        public bool IsNew { get; private set; }
        public bool IsFeatured { get; private set; }
        public bool CanCustomize { get; private set; }
        public int Stock { get; private set; }
        public string MetaDescription { get; private set; }
        public string Title { get; private set; }
        public decimal? Discount { get; private set; }
        public bool IsUnavailable { get; private set; }

        public ICollection<CatalogAttribute> CatalogAttributes { get; set; }
        public ICollection<CatalogReference>  CatalogReferences { get; set; }
        public ICollection<CatalogPicture> CatalogPictures { get; set; }
        public ICollection<CatalogCategory> CatalogCategories { get; set; } = new List<CatalogCategory>();

        public CatalogItem(int catalogIlustrationId,
            int catalogTypeId,
            bool showOnShop)
        {
            CatalogIllustrationId = catalogIlustrationId;
            CatalogTypeId = catalogTypeId;
            ShowOnShop = showOnShop;
        }

        public void UpdateName(string newName)
        {
            Name = newName;
        }
        public void UpdateSlug(string newSlug)
        {
            Slug = newSlug;
        }

        public void UpdateMainPicture(string newPictureUri)
        {
            PictureUri = newPictureUri;
        }

        public void UpdateStock(int newStock)
        {
            Stock = newStock;
        }

        public void UpdateSku(string newSku)
        {
            Sku = newSku;
        }

        public void UpdateCatalogItem(string name, string slug, string description, int catalogIllustrationId, int catalogTypeId, string pictureUri, decimal? price, decimal? discount, bool isFeatured, bool isNew, bool showOnShop, bool canCustomize, bool isUnavailable, string sku, int stock, string metaDescription, string title)
        {
            Name = name;
            Slug = slug;
            Description = description;            
            CatalogIllustrationId = catalogIllustrationId;
            CatalogTypeId = catalogTypeId;
            if(!string.IsNullOrEmpty(pictureUri))
                PictureUri = pictureUri;
            Price = price;
            Discount = discount;
            IsFeatured = isFeatured;
            IsNew = isNew;
            ShowOnShop = showOnShop;
            CanCustomize = canCustomize;
            IsUnavailable = isUnavailable;
            Sku = sku;
            Stock = stock;
            MetaDescription = metaDescription;
            Title = title;
        }

        public void UpdateFlags(int checkboxType, bool value)
        {
            if (checkboxType == 1)
                ShowOnShop = value;
            else if (checkboxType == 2)
                IsNew = value;
            else if (checkboxType == 3)
                IsFeatured = value;
            else if (checkboxType == 4)
                CanCustomize = value;
            else if (checkboxType == 5)
                IsUnavailable = value;
        }
    }
}
