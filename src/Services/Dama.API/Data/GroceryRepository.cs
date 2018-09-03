using ApplicationCore.Entities;
using Dama.API.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dama.API.Data
{
    public class GroceryRepository : EfGroceryRepository<Basket>, IGroceryRepository
    {
        private readonly GroceryContext _groceryContext;

        public GroceryRepository(GroceryContext groceryContext) : base(groceryContext)
        {
            _groceryContext = groceryContext;
            //_groceryContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public async Task<(List<CatalogItem>, long)> GetCatalogItemsAsync(int? catalogTypeId = null, int? catalogCategoryId = null, int? pageSize = null, int? pageIndex = null)
        {
            if (!catalogTypeId.HasValue && !catalogCategoryId.HasValue)
            {
                var totalItems = await _groceryContext.CatalogItems
                    .AsNoTracking()
                    .LongCountAsync();

                var itemsOnPage = await _groceryContext.CatalogItems
                   .Include(c => c.CatalogAttributes)
                   .Include(c => c.CatalogType)
                   .OrderBy(c => c.IsFeatured)
                   .Take(12)
                   .AsNoTracking()
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
                    .AsNoTracking()
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
            return await _groceryContext.CatalogItems
                .SingleOrDefaultAsync(ci => ci.Id == id);
        }

        public async Task<List<CatalogType>> GetCatalogTypesAsync(int? categoryId = null)
        {
            if (!categoryId.HasValue)
                return await _groceryContext.CatalogTypes
                    .AsNoTracking()
                    .ToListAsync();
            else
            {
                return await _groceryContext.CatalogItems
                   .Include(x => x.CatalogType)
                   .Include(x => x.CatalogCategories)
                   .Where(x => x.CatalogCategories.Any(c => c.CategoryId == categoryId))
                   .Select(x => x.CatalogType)
                   .Distinct()
                   .AsNoTracking()
                   .ToListAsync();
            }
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _groceryContext.Categories
                .AsNoTracking()
                .ToListAsync();
        }

        public List<CatalogItem> GetCatalogItemsByIds(IEnumerable<int> ids)
        {
            return _groceryContext.CatalogItems
                .Where(ci => ids.Contains(ci.Id))
                .AsNoTracking()
                .ToList();
        }

        public async Task<Basket> AddBasketItemAsync(int id, BasketItem item, int? option1 = null, int? option2 = null, int? option3 = null)
        {
            var basket = await _groceryContext.Baskets
                 .Include(b => b.Items)
                 .FirstOrDefaultAsync(x => x.Id == id);

            //basket.AddItem(item.CatalogItemId, item.UnitPrice, item.Quantity, item.Details.Select(x => x.CatalogAttributeId).ToList());
            basket.AddItem(item.CatalogItemId, item.UnitPrice, item.Quantity, option1, option2, option3);

            await _groceryContext.SaveChangesAsync();

            return basket;
        }

        public async Task DeleteBasketItemAsync(int basketItemId)
        {
            var basketItem = await _groceryContext.BasketItems.FindAsync(basketItemId);
            if (basketItem != null)
                _groceryContext.BasketItems.Remove(basketItem);
        }

        public Basket GetByIdWithItems(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<Basket> GetByIdWithItemsAsync(int id)
        {
            return _groceryContext.Baskets
               .Include(b => b.Items)
               .AsNoTracking()
               .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}