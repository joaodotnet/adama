using ApplicationCore.Entities.OrderAggregate;
using Ardalis.Specification;

namespace ApplicationCore.Specifications
{
    public class CustomerOrdersWithItemsSpecification : Specification<Order>
    {
        public CustomerOrdersWithItemsSpecification(string buyerId)
        {
            Query
                .Include(o => o.OrderItems)
                .Include($"{nameof(Order.OrderItems)}.{nameof(OrderItem.ItemOrdered)}")
                .Where(o => o.BuyerId == buyerId);
        }
    }
}
