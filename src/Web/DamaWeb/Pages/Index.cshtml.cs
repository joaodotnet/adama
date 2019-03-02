using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DamaWeb.ViewModels;
using DamaWeb.Interfaces;
using System.Linq;

namespace DamaWeb.Pages
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

        [ViewData]
        public string MetaDescription { get; set; }
        [ViewData]
        public string Title { get; set; }

        public async Task OnGet(CatalogIndexViewModel catalogModel)
        {
            //Shop Stuff
             CatalogModel = await _catalogService.GetCatalogItems(0, null, null, null, null);           

            //DamaStuff
            var config = await _shopService.GetDamaHomePageConfig();
            MetaDescription = config.MetaDescription;
            Title = config.Title;
            CatalogModel.Banners = config.Banners;
        }
    }
}
