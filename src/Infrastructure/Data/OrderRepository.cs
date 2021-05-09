using ApplicationCore.Entities.OrderAggregate;
using ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class OrderRepository : EfRepository<Order>, IOrderRepository
    {
        public OrderRepository(DamaContext dbContext) : base(dbContext)
        {
        }

        public Order GetByIdWithItems(int id)
        {
            return _dbContext.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ItemOrdered)
                .Include(o => o.OrderItems)
                //.Include("OrderItems.ItemOrdered")
                .FirstOrDefault(x => x.Id == id);
        }

        public Task<Order> GetByIdWithItemsAsync(int id)
        {
            return _dbContext.Orders
                .Include(o => o.OrderItems)
                //.Include("OrderItems.ItemOrdered")
                    .ThenInclude(oi => oi.ItemOrdered)
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
