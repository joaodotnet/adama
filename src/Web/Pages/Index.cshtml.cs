using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DamaShopWeb.Web.ViewModels;
using DamaShopWeb.Web.Interfaces;
using System.Linq;

namespace DamaShopWeb.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ICatalogService _catalogService;
        private readonly IShopService _shopService;

        public IndexModel(ICatalogService catalogService, IShopService shopService)
        {
            _catalogService = catalogService;
            _shopService = shopService;
        }

        public CatalogIndexViewModel CatalogModel { get; set; } = new CatalogIndexViewModel();

        //public async Task OnGet(CatalogIndexViewModel catalogModel, int? pageId)
        //{            
        //    //Shop Stuff
        //    CatalogModel = await _catalogService.GetCatalogItems(pageId ?? 0, Constants.ITEMS_PER_PAGE, catalogModel.BrandFilterApplied, catalogModel.TypesFilterApplied);

        //    //DamaStuff
        //    CatalogModel.Banners = await _shopService.GetMainBanners();
        //}
        public async Task OnGet(CatalogIndexViewModel catalogModel)
        {
            //Shop Stuff
             CatalogModel = await _catalogService.GetCatalogItems(0, null, null, null);           

            //DamaStuff
            CatalogModel.Banners = await _shopService.GetMainBanners();
        }
    }
}
