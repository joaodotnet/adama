using DamaNoJornal.Core.Models.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DamaNoJornal.Core.Services.Order.Models
{
    public class OrderEntity
    {
        public int Id { get; set; }
        public string BuyerId { get; set; }
        public int? TaxNumber { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public AddressEntity ShipToAddress { get; set; }
        public AddressEntity BillingToAddress { get; set; }
        public decimal ShippingCost { get; set; }
        public Boolean UseBillingSameAsShipping { get; set; }
        public OrderStatus OrderState { get; set; }
        public List<OrderItemEntity> OrderItems { get; set; }
        public string CustomerEmail { get; set; }
        public string SalesInvoiceNumber { get; set; }
    }
}
