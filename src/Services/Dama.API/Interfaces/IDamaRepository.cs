using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dama.API.Interfaces
{
    public interface IDamaRepository: IAsyncRepository<Basket>, IBasketRepository
    {
        Task<(List<CatalogItem>, long)> GetCatalogItemsAsync(int? catalogTypeId = null, int? catalogCategoryId = null, int? pageSize = null, int? pageIndex = null);
        Task<CatalogItem> GetCatalogItemAsync(int id);
        Task<List<CatalogType>> GetCatalogTypesAsync(int? categoryId = null);
        Task<List<Category>> GetCategoriesAsync();
        List<CatalogItem> GetCatalogItemsByIds(IEnumerable<int> ids);
        Task DeleteBasketItemAsync(int basketItemId);
    }
}