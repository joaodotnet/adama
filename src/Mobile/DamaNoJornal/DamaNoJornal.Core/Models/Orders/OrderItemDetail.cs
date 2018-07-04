using DamaNoJornal.Core.Models.Catalog;

namespace DamaNoJornal.Core.Models.Orders
{
    public class OrderItemDetail
    {
        public int Id { get; set; }
        public AttributeType AttributeType { get; set; }
        public string AttributeName { get; set; }
    }
}