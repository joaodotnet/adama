﻿using ApplicationCore.Entities;
using System.ComponentModel.DataAnnotations;

namespace Backoffice.ViewModels
{
    public class ProductAttributeViewModel
    {
        public int Id { get; set; }        
        [Display(Name = "Tipo")]
        public CatalogAttributeType Type { get; set; }        
        [StringLength(100)]
        [Display(Name = "Nome")]
        public string Name { get; set; }
        [Display(Name = "Preço")]
        public decimal? Price { get; set; }
        public int CatalogItemId { get; set; }
        public ProductViewModel CatalogItem { get; set; }
        public int? ReferenceCatalogItemId { get; set; }
        [Display(Name = "Produto Referência")]
        public ProductViewModel ReferenceCatalogItem { get; set; }
        public bool ToRemove { get; set; }
        [Display(Name = "SKU")]
        public string Sku { get; set; }
    }
}