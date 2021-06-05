using System;
using System.Collections.Generic;
using System.Linq;
using ApplicationCore.Interfaces;

namespace ApplicationCore.Entities
{
    public class CatalogItem : BaseEntity, IAggregateRoot
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
        public CatalogIllustration CatalogIllustration { get; private set; }
        public bool ShowOnShop { get; private set; }
        public bool IsNew { get; private set; }
        public bool IsFeatured { get; private set; }
        public bool CanCustomize { get; private set; }
        public int Stock { get; private set; }
        public string MetaDescription { get; private set; }
        public string Title { get; private set; }
        public decimal? Discount { get; private set; }
        public bool IsUnavailable { get; private set; }

        private readonly List<CatalogAttribute> _attributes = new();
        public IReadOnlyCollection<CatalogAttribute> Attributes => _attributes.AsReadOnly();
        private readonly List<CatalogReference> _references = new();
        public IReadOnlyCollection<CatalogReference>  References => _references.AsReadOnly();
        private readonly List<CatalogPicture> _pictures = new();
        public IReadOnlyCollection<CatalogPicture> Pictures => _pictures.AsReadOnly();
        private readonly List<CatalogCategory> _categories = new();
        public IReadOnlyCollection<CatalogCategory> Categories => _categories.AsReadOnly();   

        //For EF
        public CatalogItem()
        {
        }

        public CatalogItem(string name, string slug, decimal? price, string pictureUri, bool showOnShop, bool isFeatured, bool isNew, bool canCustomize,  int catalogIllustrationId, int catalogTypeId)
        {
            Name = name;
            Slug = slug;
            Price = price;
            PictureUri = pictureUri;
            ShowOnShop = showOnShop;
            IsNew = isNew;
            CanCustomize = canCustomize;
            CatalogIllustrationId = catalogIllustrationId;
            CatalogTypeId = catalogTypeId;
        }

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

        public void AddPicture(CatalogPicture catalogPicture)
        {
            _pictures.Add(catalogPicture);
        }

        public void AddCategory(int categoryId)
        {
            _categories.Add(new CatalogCategory(categoryId));
        }
        public void AddReference(string labelDescription, int referenceCatalogItemId)
        {
            _references.Add(new CatalogReference(labelDescription, referenceCatalogItemId));
        }
        public void AddAttribute(CatalogAttribute attribute)
        {
            _attributes.Add(attribute);
        }

        public void RemoveAttribute(CatalogAttribute attribute)
        {
            _attributes.Remove(attribute);
        }

        public void UpdateAttribute(int id, AttributeType type, string name, int stock)
        {
            var attribute = _attributes.SingleOrDefault(x => x.Id == id);
            if(attribute != null)
            {
                attribute.UpdateInfo(type, name, stock);
            }
        }
    }
}
