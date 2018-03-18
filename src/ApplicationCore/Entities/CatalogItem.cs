﻿using System.Collections.Generic;

namespace ApplicationCore.Entities
{
    public class CatalogItem : BaseEntity
    {
        public string Sku { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string PictureUri { get; set; }
        public int CatalogTypeId { get; set; }
        public CatalogType CatalogType { get; set; }
        public int CatalogIllustrationId { get; set; }
        public CatalogIllustration CatalogIllustration { get; set; }
        public bool ShowOnShop { get; set; }
        public bool IsNew { get; set; }
        public bool IsFeatured { get; set; }

        public ICollection<CatalogAttribute> CatalogAttributes { get; set; }
        public ICollection<CatalogAttribute> ReferenceCatalogAttributes { get; set; }
        public ICollection<CatalogPicture> CatalogPictures { get; set; }
        public ICollection<CatalogCategory> CatalogCategories { get; set; } = new List<CatalogCategory>();
    }
}