using Microsoft.AspNetCore.Mvc.Rendering;
using SalesWeb.Interfaces;
using SalesWeb.ViewModels;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Specifications;

namespace SalesWeb.Services
{
    public class CachedCatalogService : ICatalogService
    {
        private readonly IMemoryCache _cache;
        private readonly CatalogService _catalogService;
        private static readonly string _typesKey = "types";
        private static readonly string _itemsKeyTemplate = "items-{0}-{1}-{2}-{3}-{4}";
        private static readonly string _categoryItemsKeyTemplate = "categories-items-{0}-{1}-{2}";
        private static readonly string _categoryItemKeyTemplate = "item-{0}";
        private static readonly string _itemsByTagKeyTemplate = "tag-{0}-{1}-{2}-{3}-{4}-{5}";
        private static readonly string _itemsBySearchKeyTemplate = "search-{0}-{1}-{2}-{3}-{4}";
        private static readonly string _menuKeyTemplate = "damamenu";
        private static readonly string _categoryKeyTemplate = "category-{0}";
        private static readonly string _categoryTypesKeyTemplate = "category-types-{0}";
        private static readonly TimeSpan _defaultCacheDuration = TimeSpan.FromSeconds(30);

        public CachedCatalogService(IMemoryCache cache,
            CatalogService catalogService)
        {
            _cache = cache;
            _catalogService = catalogService;
        }

        //public async Task<IEnumerable<SelectListItem>> GetIllustrations()
        //{
        //    return await _cache.GetOrCreateAsync(_brandsKey, async entry =>
        //            {
        //                entry.SlidingExpiration = _defaultCacheDuration;
        //                return await _catalogService.GetIllustrations();
        //            });
        //}

        public async Task<CatalogIndexViewModel> GetCatalogItems(int pageIndex, int? itemsPage, int? brandID, int? typeId, int? categoryId)
        {
            string cacheKey = String.Format(_itemsKeyTemplate, pageIndex, itemsPage, brandID, typeId, categoryId);
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = _defaultCacheDuration;
                return await _catalogService.GetCatalogItems(pageIndex, itemsPage, brandID, typeId, categoryId);
            });
        }

        public async Task<IEnumerable<SelectListItem>> GetTypes()
        {
            return await _cache.GetOrCreateAsync(_typesKey, async entry =>
            {
                entry.SlidingExpiration = _defaultCacheDuration;
                return await _catalogService.GetTypes();
            });
        }

        public async Task<CatalogIndexViewModel> GetCategoryCatalogItems(int categoryId, int pageIndex, int? itemsPage)
        {
            string cacheKey = String.Format(_categoryItemsKeyTemplate, categoryId, pageIndex, itemsPage);
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = _defaultCacheDuration;
                return await _catalogService.GetCategoryCatalogItems(categoryId, pageIndex, itemsPage);
            });
        }

        public async Task<ProductViewModel> GetCatalogItem(string id)
        {
            string cacheKey = String.Format(_categoryItemKeyTemplate, id);
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = _defaultCacheDuration;
                return await _catalogService.GetCatalogItem(id);
            });
        }

        public async Task<CatalogIndexViewModel> GetCatalogItemsByTag(int pageIndex, int? itemsPage, string tagName, TagType? tagType, int? typeId, int? illustrationId)
        {
            string cacheKey = String.Format(_itemsByTagKeyTemplate, tagName, tagType, pageIndex, itemsPage, typeId, illustrationId);
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = _defaultCacheDuration;
                return await _catalogService.GetCatalogItemsByTag(pageIndex,itemsPage,tagName,tagType,typeId,illustrationId);
            });
        }
        public async Task<CatalogIndexViewModel> GetCatalogItemsBySearch(int pageIndex, int? itemsPage, string searchfor, int? typeId, int? illustrationId)
        {
            string cacheKey = String.Format(_itemsBySearchKeyTemplate, searchfor, pageIndex, itemsPage, typeId, illustrationId);
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = _defaultCacheDuration;
                return await _catalogService.GetCatalogItemsBySearch(pageIndex, itemsPage, searchfor, typeId, illustrationId);
            });
        }

        public async Task<List<MenuItemComponentViewModel>> GetMenuViewModel()
        {
            string cacheKey = _menuKeyTemplate;
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = _defaultCacheDuration;
                return await _catalogService.GetMenuViewModel();
            });
        }

        public async Task<(int, string)?> GetCatalogType(string type)
        {
            string cacheKey = String.Format(_categoryTypesKeyTemplate, type);
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = _defaultCacheDuration;
                return await _catalogService.GetCatalogType(type);
            });
        }

        public async Task<(int, string)?> GetCategory(string name)
        {
            string cacheKey = String.Format(_categoryKeyTemplate, name);
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = _defaultCacheDuration;
                return await _catalogService.GetCategory(name);
            });
        }
    }
}
