using ApplicationCore.Entities.OrderAggregate;
using Ardalis.Specification;

namespace ApplicationCore.Specifications
{
    public class OrdersSpecification : Specification<Order>
    {       
        public OrdersSpecification(string buyerId)
        {
            Query
                .Include(o => o.OrderItems)
                .Where(o => o.BuyerId == buyerId);

        }

        public OrdersSpecification(int orderId)
            : base()
        {
            Query
                .Include(o => o.OrderItems)
                .Where(o => o.Id == orderId);
        }
    }
}
