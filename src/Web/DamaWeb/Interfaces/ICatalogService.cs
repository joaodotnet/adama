using Microsoft.AspNetCore.Mvc.Rendering;
using DamaWeb.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Specifications;

namespace DamaWeb.Interfaces
{
    public interface ICatalogService
    {
        Task<CatalogIndexViewModel> GetCatalogItems(int pageIndex, int? itemsPage, int? illustrationId, int? typeId, int? categoryId, string onlyAvailable = null);
        Task<CategoryViewModel> GetCategoryCatalogItems(string categoryUrlName, int pageIndex, int? itemsPage, string onlyAvailable = null);
        Task<IEnumerable<SelectListItem>> GetIllustrations();
        Task<IEnumerable<SelectListItem>> GetTypes();
        Task<ProductViewModel> GetCatalogItem(string sku);
        Task<CatalogIndexViewModel> GetCatalogItemsByTag(int pageIndex, int? itemsPage, string tagName, TagType? tagType, string onlyAvailable = null);
        Task<CatalogIndexViewModel> GetCatalogItemsBySearch(int pageIndex, int? itemsPage, string searchFor, string onlyAvailable = null);
        Task<List<MenuItemComponentViewModel>> GetMenuViewModel();
        Task<CatalogTypeViewModel> GetCatalogTypeItemsAsync(string cat, string type, int pageIndex, int itemsPage, string onlyAvailable = null);
        Task<string> GetSlugFromSkuAsync(string sku);
    }
}
