using ApplicationCore.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.ViewModels
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }
        public string ProductTitle { get; set; }
        public string ProductDescription { get; set; }
        public decimal ProductBasePrice { get; set; }
        public List<string> ProductImagesUri { get; set; }
        public List<ProductAttributeViewModel> Attributes { get; set; }
        public int ProductQuantity { get; set; } = 1;
        [Display(Name = "Referência:")]
        public string ProductSKU { get; set; }
        public List<LinkViewModel> Categories { get; set; }
        public List<LinkViewModel> Tags { get; set; }

    }

    public class LinkViewModel
    {
        public string Uri { get; set; }
        public string Name { get; set; }
    }

    public class ProductAttributeViewModel
    {
        public CatalogAttributeType AttributeType { get; set; }
        public string Label { get; set; }
        public int? DefaultValue { get; set; }
        public string DefaultText { get; set; }
        public IEnumerable<SelectListItem> Items { get; set; }
        public int Selected { get; set; }
        public List<AttributeViewModel> Attributes { get; set; }
    }

    public class AttributeViewModel
    {
        public int Id { get; set; }
        public decimal? Price { get; set; }
        public string Sku { get; set; }
    }
}
