using Microsoft.AspNetCore.Mvc.Rendering;
using Web.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Web.Interfaces
{
    public interface ICatalogService
    {
        Task<CatalogIndexViewModel> GetCatalogItems(int pageIndex, int? itemsPage, int? illustrationId, int? typeId);
        Task<CatalogIndexViewModel> GetCategoryCatalogItems(int categoryId);
        Task<IEnumerable<SelectListItem>> GetBrands();
        Task<IEnumerable<SelectListItem>> GetTypes();
        Task<ProductViewModel> GetCatalogItem(string sku);
        Task<decimal?> GetAttributePrice(int attributeId);
    }
}
