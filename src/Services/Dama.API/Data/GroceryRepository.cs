using ApplicationCore.Entities;
using Dama.API.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dama.API.Data
{
    public class GroceryRepository : IGroceryRepository
    {
        private readonly GroceryContext _groceryContext;

        public GroceryRepository(GroceryContext groceryContext)
        {
            _groceryContext = groceryContext;
            _groceryContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public async Task<(List<CatalogItem>, long)> GetCatalogItemsAsync(int? catalogTypeId = null, int? catalogCategoryId = null, int? pageSize = null, int? pageIndex = null)
        {
            if (!catalogTypeId.HasValue && !catalogCategoryId.HasValue)
            {
                var totalItems = await _groceryContext.CatalogItems
                    .LongCountAsync();

                var itemsOnPage = await _groceryContext.CatalogItems
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
                var root = (IQueryable<CatalogItem>)_groceryContext.CatalogItems
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

                return (itemsOnPage, totalItems);
            }

        }

        public async Task<CatalogItem> GetCatalogItemAsync(int id)
        {
            return await _groceryContext.CatalogItems.SingleOrDefaultAsync(ci => ci.Id == id);
        }

        public async Task<List<CatalogType>> GetCatalogTypesAsync(int? categoryId = null)
        {
            if (!categoryId.HasValue)
                return await _groceryContext.CatalogTypes
                    .ToListAsync();
            else
            {
                return await _groceryContext.CatalogItems
                   .Include(x => x.CatalogType)
                   .Include(x => x.CatalogCategories)
                   .Where(x => x.CatalogCategories.Any(c => c.CategoryId == categoryId))
                   .Select(x => x.CatalogType)
                   .Distinct()
                   .ToListAsync();
            }
        }

        //public async Task<List<Category>> GetCatalogCategoriesAsync()
        //{
        //    return await _groceryContext.Categories
        //        .ToListAsync();
        //}

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _groceryContext.Categories
                 .ToListAsync();
        }

        public List<CatalogItem> GetCatalogItemsByIds(IEnumerable<int> ids)
        {
            return _groceryContext.CatalogItems.Where(ci => ids.Contains(ci.Id)).ToList();
        }
    }
}