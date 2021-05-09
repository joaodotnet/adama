using ApplicationCore.Entities;
using System.Collections.Generic;

namespace ApplicationCore.Entities.OrderAggregate
{

    public class OrderItem : BaseEntity
    {
        public CatalogItemOrdered ItemOrdered { get; private set; }
        public CustomizeItemOrdered CustomizeItem { get; private set; }
        public decimal UnitPrice { get; private set; }
        public int Units { get; private set; }
        public int? CatalogAttribute1 { get; private set; }
        public int? CatalogAttribute2 { get; private set; }
        public int? CatalogAttribute3 { get; private set; }
        public string CustomizeName { get; private set; }
        public string CustomizeSide { get; private set; }

        //public ICollection<OrderItemDetail> Details { get; set; } = new List<OrderItemDetail>();

        protected OrderItem()
        {
        }
        public OrderItem(CatalogItemOrdered itemOrdered, decimal unitPrice, int units, int? option1 = null, int? option2 = null, int? option3 = null, string customizeName = null, string customizeSide = null, CustomizeItemOrdered customizeItem = null)
        {
            ItemOrdered = itemOrdered;
            CustomizeItem = customizeItem;
            UnitPrice = unitPrice;
            Units = units;
            CatalogAttribute1 = option1;
            CatalogAttribute2 = option2;
            CatalogAttribute3 = option3;
            CustomizeName = customizeName;
            CustomizeSide = customizeSide;
        }

        public void UpdateItemPrice(decimal newPrice)
        {
            this.UnitPrice = newPrice;
        }
    }
}
