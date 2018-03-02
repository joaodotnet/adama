using ApplicationCore.Entities;

namespace ApplicationCore.Entities.OrderAggregate
{

    public class OrderItemDetail : BaseEntity
    {
        public int OrderItemId { get; set; }
        public OrderItem OrderItem { get; set; }
        public CatalogAttributeType AttributeType { get; set; }
        public string AttributeCode { get; set; }
        public string AttributeName { get; set; }

        //protected OrderItemDetail()
        //{
        //}
        //public OrderItemDetail(CatalogItemAttributeOrdered itemAttributed, CatalogAttributeType type, string code, string name)
        //{
        //    ItemAttribute = itemAttributed;
        //    AttributeType = type;
        //    AttributeCode = code;
        //    AttributeName = name;
        //}
    }
}
