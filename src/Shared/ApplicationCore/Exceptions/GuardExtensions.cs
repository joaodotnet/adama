using ApplicationCore.Exceptions;
using ApplicationCore.Entities.BasketAggregate;

namespace Ardalis.GuardClauses
{
    public static class FooGuard
    {
        public static void NullBasket(this IGuardClause guardClause, int basketId, Basket basket)
        {
            if (basket == null)
                throw new BasketNotFoundException(basketId);
        }
    }
}