using ApplicationCore.Entities;
using ApplicationCore.Entities.BasketAggregate;
using ApplicationCore.Entities.OrderAggregate;
using ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class BasketRepository : IBasketRepository
    {
        private readonly DamaContext _dbContext;

        public BasketRepository(DamaContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Basket GetByIdWithItems(int id)
        {
            return _dbContext.Baskets
                .Include(b => b.Items)
                .FirstOrDefault(x => x.Id == id);
        }

        public Task<Basket> GetByIdWithItemsAsync(int id)
        {
            return _dbContext.Baskets
                .Include(b => b.Items)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
      
        public async Task<Basket> AddBasketItemAsync(int id, BasketItem item, int? option1 = null, int? option2 = null, int? option3 = null)
        {
           var basket = await _dbContext.Baskets
                .Include(b => b.Items)
                .FirstOrDefaultAsync(x => x.Id == id);

            //basket.AddItem(item.CatalogItemId, item.UnitPrice, item.Quantity, item.Details.Select(x => x.CatalogAttributeId).ToList());
            basket.AddItem(item.CatalogItemId, item.UnitPrice, item.Quantity, option1, option2, option3);

            await _dbContext.SaveChangesAsync();

            return basket;
        }
    }
}
