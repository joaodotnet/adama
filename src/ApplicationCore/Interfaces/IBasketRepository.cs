using ApplicationCore.Entities;
using ApplicationCore.Entities.BasketAggregate;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{

    public interface IBasketRepository
    {
        Basket GetByIdWithItems(int id);
        Task<Basket> GetByIdWithItemsAsync(int id);
        Task<Basket> AddBasketItemAsync(int id, BasketItem value, int? option1 = null, int? option2 = null, int? option3 = null);
    }
}
