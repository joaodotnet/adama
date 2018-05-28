using ApplicationCore.Entities;
using ApplicationCore.Entities.OrderAggregate;
using ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class BasketRepository : EfRepository<Basket>, IBasketRepository
    {
        public BasketRepository(DamaContext dbContext) : base(dbContext)
        {
        }

        

        public Basket GetByIdWithItems(int id)
        {
            return _dbContext.Baskets
                .Include(b => b.Items)
                .Include("Items.Details")
                .Include("Items.Details.CatalogAttribute")
                .FirstOrDefault(x => x.Id == id);
        }

        public Task<Basket> GetByIdWithItemsAsync(int id)
        {
            return _dbContext.Baskets
                .Include(b => b.Items)
                .Include("Items.Details")
                .Include("Items.Details.CatalogAttribute")
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Basket> UpdateBasketAsync(Basket value)
        {
            var basket = await _dbContext.Baskets
                .Include(b => b.Items)
                .Include("Items.Details")
                .Include("Items.Details.CatalogAttribute")
                .FirstOrDefaultAsync(x => x.BuyerId == value.BuyerId);            

            if(basket == null)
            {
                basket = value;
                _dbContext.Baskets.Add(value);
            }
            else
            {
                //Remove all
                basket.RemoveAllItems();
                //Add again
                foreach (var item in value.Items)
                {
                    basket.AddItem(item.CatalogItemId, item.UnitPrice, item.Quantity);
                }
            }
            //await _dbContext.SaveChangesAsync();

            //for (int i = 0; i < basket.Items.Count; i++)
            //{
            //    basket.RemoveItem(i);
            //}

           
            await _dbContext.SaveChangesAsync();
            return basket;
        }

        public async Task DeleteBasketAsync(int id)
        {
            _dbContext.Baskets.Remove(await GetByIdAsync(id));
        }

        public async Task<Basket> AddBasketItemAsync(int id, BasketItem item)
        {
           var basket = await _dbContext.Baskets
                .Include(b => b.Items)
                .Include("Items.Details")
                .Include("Items.Details.CatalogAttribute")
                .FirstOrDefaultAsync(x => x.Id == id);

            basket.AddItem(item.CatalogItemId, item.UnitPrice, item.Quantity);

            await _dbContext.SaveChangesAsync();

            return basket;
        }
    }
}
