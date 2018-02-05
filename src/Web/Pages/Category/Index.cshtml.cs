using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DamaShopWeb.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.Interfaces;
using Web.ViewModels;

namespace Web.Pages.Category
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

        public CategoryViewModel CategoryModel { get; set; } = new CategoryViewModel();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var cat = await _shopService.GetCategory(id);
            if (cat == null)
                return NotFound();
            else
                CategoryModel.CategoryName = cat.Name;

            CategoryModel.CatalogModel = await _catalogService.GetCatalogItems(0, null, cat.Id, null);
            CategoryModel.CatalogTypes = CategoryModel.CatalogModel.CatalogItems.Select(x => (x.CatalogTypeCode,x.CatalogTypeName)).Distinct().ToList();
            CategoryModel.CategoryUrlName = id.ToLower();

            return Page();
        }
    }
}