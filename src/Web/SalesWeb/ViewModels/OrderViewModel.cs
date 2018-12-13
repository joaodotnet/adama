using System;
using ApplicationCore.Entities.OrderAggregate;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SalesWeb.ViewModels
{
    public class OrderViewModel
    {
        public int OrderNumber { get; set; }
        public long InvoiceId { get; set; }
        public string InvoiceNr { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy - HH:mm}")]
        public DateTimeOffset OrderDate { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
        public decimal ShippingCost { get; set; }

        public Address ShippingAddress { get; set; }

        public Address BillingAddress { get; set; }

        public List<OrderItemViewModel> OrderItems { get; set; } = new List<OrderItemViewModel>();
    }
}
