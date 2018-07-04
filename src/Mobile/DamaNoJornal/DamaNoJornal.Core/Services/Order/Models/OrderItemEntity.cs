using DamaNoJornal.Core.Models.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DamaNoJornal.Core.Services.Order.Models
{
    public class OrderItemEntity
    {
        public int Id { get; set; }
        public CatalogItemOrdered ItemOrdered { get; set; }
        public decimal UnitPrice { get; set; }
        public int Units { get; set; }
        public ICollection<OrderItemDetail> Details { get; set; } = new List<OrderItemDetail>();
    }
}
