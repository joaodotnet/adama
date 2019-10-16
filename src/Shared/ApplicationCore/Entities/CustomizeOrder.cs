using ApplicationCore.Entities.OrderAggregate;
using ApplicationCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class CustomizeOrder : BaseEntity, IAggregateRoot
    {
        public string BuyerId { get; set; }
        public string BuyerName { get; set; }
        public string BuyerContact { get; set; }
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;
        public OrderStateType OrderState { get; set; }

        public CatalogItemOrdered ItemOrdered { get; set; }
        public string Description { get; set; }
        public string Text { get; set; }
        public string AttachFileName { get; set; }
        public string Colors { get; set; }
    }
}
