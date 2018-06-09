using ApplicationCore.Entities;
using ApplicationCore.Entities.OrderAggregate;

namespace ApplicationCore.Specifications
{
    public class OrdersSpecification : BaseSpecification<Order>
    {       
        public OrdersSpecification(string buyerId)
            :base(o => o.BuyerId == buyerId)
        {
            AddInclude(o => o.OrderItems);            

        }

        public OrdersSpecification(int orderId)
            : base(o => o.Id == orderId)
        {
            AddInclude(o => o.OrderItems);

        }
    }
}
