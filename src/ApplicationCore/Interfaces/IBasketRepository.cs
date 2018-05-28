using ApplicationCore.Entities;
using ApplicationCore.Entities.OrderAggregate;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{

    public interface IBasketRepository : IRepository<Basket>, IAsyncRepository<Basket>
    {
        Basket GetByIdWithItems(int id);
        Task<Basket> GetByIdWithItemsAsync(int id);
        Task<Basket> UpdateBasketAsync(Basket value);
        Task<Basket> AddBasketItemAsync(int id, BasketItem value);
        Task DeleteBasketAsync(int id);
    }
}
