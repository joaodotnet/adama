using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DamaWeb.Interfaces;
using DamaWeb.ViewModels;

namespace DamaWeb.Pages.Category.Type
{
    public class IndexModel : PageModel
    {
        private readonly IShopService _shopService;
        private readonly ICatalogService _catalogService;
        public IndexModel(IShopService service, ICatalogService catalogService)
        {
            _shopService = service;
            _catalogService = catalogService;
        }
        [TempData]
        public string CategoryName { get; set; }
        [TempData]
        public string CatalogTypeName { get; set; }
        //public CategoryViewModel CategoryModel { get; set; } = new CategoryViewModel();
        public CatalogIndexViewModel CatalogModel { get; set; } = new CatalogIndexViewModel();

        public async Task<IActionResult> OnGetAsync(CatalogIndexViewModel catalogModel, string cat, string type, int? p)
        {
            var catalogType = await _catalogService.GetCatalogType(type);
            if (!catalogType.HasValue)
                return NotFound();
            CatalogTypeName = catalogType.Value.Item2;

            var category = await _catalogService.GetCategory(cat);
            if (!category.HasValue)
                return NotFound();          

            CatalogModel = await _catalogService.GetCatalogItems(p ?? 0, Constants.ITEMS_PER_PAGE, catalogModel.IllustrationFilterApplied, catalogType.Value.Item1,category.Value.Item1);

            return Page();
        }
    }
}