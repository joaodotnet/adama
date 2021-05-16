using ApplicationCore.Entities.OrderAggregate;
using ApplicationCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class CustomizeOrder : BaseEntity, IAggregateRoot
    {
        public string BuyerId { get; private set; }
        public string BuyerName { get; private set; }
        public string BuyerContact { get; private set; }
        public DateTimeOffset OrderDate { get; private set; } = DateTimeOffset.Now;
        public OrderStateType OrderState { get; private set; }

        public CatalogItemOrdered ItemOrdered { get; private set; }
        public string Description { get; private set; }
        public string Text { get; private set; }
        public string AttachFileName { get; private set; }
        public string Colors { get; private set; }

        public CustomizeOrder()
        {            
        }
        public CustomizeOrder(string buyerId, string buyerName, string buyerContact, string description, string text, string colors, CatalogItemOrdered itemOrdered, string attachFileName)
        {
            BuyerId = buyerId;
            BuyerName = buyerName;
            BuyerContact = buyerContact;
            Description = description;
            Text = text;
            Colors = colors;
            ItemOrdered = itemOrdered;
            AttachFileName = attachFileName;
        }

        public void UpdateOrderState(OrderStateType orderState)
        {
            OrderState = orderState;
        }
    }
}
