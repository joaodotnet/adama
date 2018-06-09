using System.Collections.Generic;

namespace ApplicationCore.Entities
{
    public class BasketItem : BaseEntity
    {
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public int CatalogItemId { get; set; }
        public List<BasketDetailItem> Details { get; set; }
    }
}
