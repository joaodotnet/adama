using Microsoft.AspNetCore.Mvc.Rendering;
using DamaWeb.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Specifications;

namespace DamaWeb.Interfaces
{
    public interface ICatalogService
    {
        Task<CatalogIndexViewModel> GetCatalogItems(int pageIndex, int? itemsPage, int? illustrationId, int? typeId, int? categoryId);
        Task<CatalogIndexViewModel> GetCategoryCatalogItems(int categoryId);
        Task<IEnumerable<SelectListItem>> GetIllustrations();
        Task<IEnumerable<SelectListItem>> GetTypes();
        Task<ProductViewModel> GetCatalogItem(string sku);
        //Task<AttributeViewModel> GetAttributeDetails(int attributeId);
        Task<CatalogIndexViewModel> GetCatalogItemsByTag(int pageIndex, int? itemsPage, string tagName, TagType? tagType, int? typeId, int? illustrationId);
        Task<CatalogIndexViewModel> GetCatalogItemsBySearch(int pageIndex, int? itemsPage, string searchFor, int? typeId, int? illustrationId);
        Task<List<MenuItemComponentViewModel>> GetMenuViewModel();
        Task<(int,string)?> GetCatalogType(string type);
        Task<(int, string)?> GetCategory(string name);
    }
}
