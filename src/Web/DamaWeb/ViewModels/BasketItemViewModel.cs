using System.Collections.Generic;

namespace DamaWeb.ViewModels
{

    public class BasketItemViewModel
    {
        public int Id { get; set; }
        public int CatalogItemId { get; set; }
        public string ProductName { get; set; }
        public string ProductName2 { get; set; }
        public string CustomizeName { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal OldUnitPrice { get; set; }
        public int Quantity { get; set; }
        public string PictureUrl { get; set; }
        public List<AttributeViewModel> Attributes { get; set; } = new List<AttributeViewModel>();
        public bool IsFromCustomize { get; set; } = false;
        public string Sku { get; set; }
        public string Slug { get; set; }
    }
}
