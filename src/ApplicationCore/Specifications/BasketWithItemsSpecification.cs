using ApplicationCore.Entities.BasketAggregate;
using Ardalis.Specification;

namespace ApplicationCore.Specifications
{
    public sealed class BasketWithItemsSpecification : Specification<Basket>, ISingleResultSpecification
    {
        public BasketWithItemsSpecification(int basketId)
        {
            Query
                .Include(b => b.Items)
                .Where(b => b.Id == basketId);
        }
        public BasketWithItemsSpecification(string buyerId, bool guestOnly = false)
        {
            Query
                .Include(b => b.Items)
                .Where(b => b.BuyerId == buyerId && ((!guestOnly && !b.IsGuest) || (guestOnly && b.IsGuest)));
        }
    }
}
