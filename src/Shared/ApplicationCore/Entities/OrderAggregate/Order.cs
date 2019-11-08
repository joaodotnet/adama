using ApplicationCore.Interfaces;
using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApplicationCore.Entities.OrderAggregate
{
    public class Order : BaseEntity, IAggregateRoot
    {
        private Order()
        {
        }

        public Order(string buyerId, string phoneNumber, int? taxNumber, Address shipToAddress, Address billingAddress, bool useBillingSameAsShipping, List<OrderItem> items, decimal shippingCost, string observations, string customerEmail = null)
        {
            ShipToAddress = shipToAddress;
            if (billingAddress != null)
                BillingToAddress = billingAddress;
            else
                BillingToAddress = new Address();
            UseBillingSameAsShipping = useBillingSameAsShipping;
            ShippingCost = shippingCost;
            _orderItems = items;
            BuyerId = buyerId;
            CustomerEmail = customerEmail;
            TaxNumber = taxNumber;
            PhoneNumber = phoneNumber;
            OrderState = items.Any(x => x.CustomizeItem?.CatalogTypeId != null) ? OrderStateType.UNDER_ANALYSIS : OrderStateType.PENDING;
            Observations = observations;
        }
        public string BuyerId { get; private set; }
        public string CustomerEmail { get; set; }
        public int? TaxNumber { get; private set; }
        public string PhoneNumber { get; private set; }
        public DateTimeOffset OrderDate { get; private set; } = DateTimeOffset.Now;
        public Address ShipToAddress { get; private set; }
        public Address BillingToAddress { get; private set; }
        public decimal ShippingCost { get; private set; }
        public bool UseBillingSameAsShipping { get; private set; }
        public OrderStateType OrderState { get; set; }
        public long? SalesInvoiceId { get; set; }
        public string SalesInvoiceNumber { get; set; }
        public long? SalesPaymentId { get; set; }
        public string Observations { get; private set; }

        // DDD Patterns comment
        // Using a private collection field, better for DDD Aggregate's encapsulation
        // so OrderItems cannot be added from "outside the AggregateRoot" directly to the collection,
        // but only through the method Order.AddOrderItem() which includes behavior.
        private readonly List<OrderItem> _orderItems = new List<OrderItem>();

        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();



        // Using List<>.AsReadOnly() 
        // This will create a read only wrapper around the private list so is protected against "external updates".
        // It's much cheaper than .ToList() because it will not have to copy all items in a new collection. (Just one heap alloc for the wrapper instance)
        //https://msdn.microsoft.com/en-us/library/e78dcd75(v=vs.110).aspx 

        public decimal SubTotal()
        {
            var subtotal = 0m;
            foreach (var item in _orderItems)
            {
                subtotal += item.UnitPrice * item.Units;
            }
            return subtotal;
        }

        public decimal Total()
        {
            var total = 0m;
            foreach (var item in _orderItems)
            {
                total += item.UnitPrice * item.Units;
            }
            return total + ShippingCost;
        }

        public void UpdateBillingInfo(int? taxNumber, string customerEmail, Address billingAddress)
        {
            this.TaxNumber = taxNumber;
            this.CustomerEmail = customerEmail;
            this.BillingToAddress = billingAddress;
        }
    }
}
