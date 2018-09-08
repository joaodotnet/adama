using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DamaNoJornal.Core.Models.Orders
{
    public class Order
    {
        public Order()
        {
            SequenceNumber = 1;
            OrderItems = new List<OrderItem>();
        }
        
        public string BuyerId { get; set; }
        public int SequenceNumber { get; set; }

        public DateTime OrderDate { get; set; }

        [JsonProperty("orderState")]
        public OrderStatus OrderStatus { get; set; }

        public string ShippingCity { get; set; }

        public string ShippingStreet { get; set; }

        public string ShippingState { get; set; }

        public string ShippingCountry { get; set; }

        public string ShippingZipCode { get; set; }

        public string BillingName { get; set; }
        public string BillingCity { get; set; }

        public string BillingStreet { get; set; }

        public string BillingCountry { get; set; }

        public string BillingPostalCode { get; set; }

        public int CardTypeId { get; set; }

        public string CardNumber { get; set; }

        public string CardHolderName { get; set; }

        public DateTime CardExpiration { get; set; }

        public string CardSecurityNumber { get; set; }

        [JsonProperty("orderItems")]
        public List<OrderItem> OrderItems { get; set; }

        public decimal ShippingCost { get; set; }

        [JsonProperty("id")]
        public int OrderNumber { get; set; }

        public string CustomerEmail { get; set; }
        public int? TaxNumber { get; set; }
        public bool CreateInvoice { get; set; } = false;
        public string ResultMessage { get; set; }

        public decimal Total
        {
            get
            {
                var total = 0m;
                foreach (var item in OrderItems)
                {
                    total += item.Total;
                }
                return total + ShippingCost;
            }
        }

        public int TotalOrderItems { get
            {
                return OrderItems.Sum(x => x.Quantity);
            }
        }
    }
}