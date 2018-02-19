using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.Interfaces;
using Web.ViewModels;

namespace Web.Pages.Product
{
    public class IndexModel : PageModel
    {
        private readonly ICatalogService _catalogService;
        public IndexModel(ICatalogService catalogService)
        {
            _catalogService = catalogService;
        }
        public ProductViewModel ProductModel { get; set; } = new ProductViewModel();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();
            ProductModel = await _catalogService.GetCatalogItem(id);
            return Page();
        }

        public async Task<JsonResult> OnGetAttributePriceAsync(int id)
        {
            var res = await _catalogService.GetAttributePrice(id);
            return new JsonResult(res);
        }
    }
}