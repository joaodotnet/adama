using ApplicationCore.Entities.BasketAggregate;

namespace ApplicationCore.Specifications
{
    public sealed class BasketWithItemsSpecification : BaseSpecification<Basket>
    {
        public BasketWithItemsSpecification(int basketId)
            :base(b => b.Id == basketId)
        {
            AddInclude(b => b.Items);
        }
        public BasketWithItemsSpecification(string buyerId, bool guestOnly = false)
            : base(b => b.BuyerId == buyerId && ((!guestOnly && !b.IsGuest) || (guestOnly && b.IsGuest)))
        {
            AddInclude(b => b.Items);
        }
    }
}
