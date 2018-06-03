using Newtonsoft.Json;
using System;

namespace DamaNoJornal.Core.Models.Orders
{
    public class OrderItem
    {
        public int ProductId { get; set; }
        public Guid? OrderId { get; set; }

        [JsonProperty("unitprice")]
        public decimal UnitPrice { get; set; }

        public string ProductName { get; set; }

        public string PictureUrl { get; set; }

        [JsonProperty("units")]
        public int Quantity { get; set; }

        public decimal Discount { get; set; }
        public decimal Total { get { return Quantity * UnitPrice; } }

        public override string ToString()
        {
            return String.Format("Product Id: {0}, Quantity: {1}", ProductId, Quantity);
        }
    }
}