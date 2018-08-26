using ApplicationCore.Entities;
using ApplicationCore.Interfaces;

namespace Dama.API.Interfaces
{
    public interface IGroceryRepository : IDamaRepository, IGroceryAsyncRepository<Basket>
    { }
}