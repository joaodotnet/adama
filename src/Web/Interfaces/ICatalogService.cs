using Microsoft.AspNetCore.Mvc.Rendering;
using DamaShopWeb.Web.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DamaShopWeb.Web.Interfaces
{
    public interface ICatalogService
    {
        Task<CatalogIndexViewModel> GetCatalogItems(int pageIndex, int itemsPage, int? brandId, int? typeId);
        Task<IEnumerable<SelectListItem>> GetBrands();
        Task<IEnumerable<SelectListItem>> GetTypes();
    }
}
