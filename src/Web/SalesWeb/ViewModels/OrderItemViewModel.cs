using System.Collections.Generic;

namespace SalesWeb.ViewModels
{
    public class OrderItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public int Units { get; set; }
        public string PictureUrl { get; set; }
        public string CustomizeName { get; set; }
        public List<OrderItemDetailViewModel> Attributes { get; set; } = new List<OrderItemDetailViewModel>();
    }
}
