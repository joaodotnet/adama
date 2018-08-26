using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Dama.API.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dama.API.Data
{
    public class DamaRepository : BasketRepository, IDamaRepository
    {
        private readonly DamaContext _damaContext;

        public DamaRepository(DamaContext damaContext) : base(damaContext)
        {
            _damaContext = damaContext;
            _damaContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public async Task<(List<CatalogItem>,long)> GetCatalogItemsAsync(int? catalogTypeId = null, int? catalogCategoryId = null, int? pageSize = null, int? pageIndex = null)
        {
            if(!catalogTypeId.HasValue && !catalogCategoryId.HasValue)
            {
                var totalItems = await _damaContext.CatalogItems
                    .LongCountAsync();

                var itemsOnPage = await _damaContext.CatalogItems
                   .Include(c => c.CatalogAttributes)
                   .Include(c => c.CatalogType)
                   .OrderBy(c => c.IsFeatured)
                   .Take(12)
                   .ToListAsync();

                itemsOnPage.ForEach(x => x.Price = x.Price > 0 ? x.Price : x.CatalogType.Price);

                return (itemsOnPage, totalItems);
            }
            else
            {
                var root = (IQueryable<CatalogItem>)_damaContext.CatalogItems
                            .Include(x => x.CatalogCategories)
                            .Include(x => x.CatalogType);

                if (catalogTypeId.HasValue)
                {
                    root = root.Where(ci => ci.CatalogTypeId == catalogTypeId);
                }

                if (catalogCategoryId.HasValue)
                {
                    root = root.Where(ci => ci.CatalogCategories.Any(x => x.CategoryId == catalogCategoryId));
                }

                var totalItems = await root
                    .LongCountAsync();

                var itemsOnPage = await root
                    .OrderBy(c => c.Name)
                    .Skip(pageSize.Value * pageIndex.Value)
                    .Take(pageSize.Value)
                    .ToListAsync();

                return (itemsOnPage,totalItems);
            }
            
        }

        public async Task<CatalogItem> GetCatalogItemAsync(int id)
        {
            return await _damaContext.CatalogItems.SingleOrDefaultAsync(ci => ci.Id == id);
        }

        public async Task<List<CatalogType>> GetCatalogTypesAsync(int? categoryId = null)
        {
            if(!categoryId.HasValue)
                return await _damaContext.CatalogTypes
                    .ToListAsync();
            else
                return await _damaContext.CatalogTypeCategories
                   .Include(x => x.CatalogType)
                   .Where(x => x.CategoryId == categoryId)
                   .Select(x => x.CatalogType)
                   .ToListAsync();
        }

        //public async Task<List<Category>> GetCatalogCategoriesAsync()
        //{
        //    return await _damaContext.Categories
        //        .ToListAsync();
        //}

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _damaContext.Categories
               .ToListAsync();
        }

        public List<CatalogItem> GetCatalogItemsByIds(IEnumerable<int> ids)
        {
            return _damaContext.CatalogItems.Where(ci => ids.Contains(ci.Id)).ToList();
        }

        public async Task DeleteBasketItemAsync(int basketItemId)
        {
            var basketItem = await _damaContext.BasketItems.FindAsync(basketItemId);
            if (basketItem != null)
                _damaContext.BasketItems.Remove(basketItem);
        }
    }
}