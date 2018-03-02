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
    }
}
