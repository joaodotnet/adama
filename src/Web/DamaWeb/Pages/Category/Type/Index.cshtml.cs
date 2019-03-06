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
        private readonly ICatalogService _catalogService;
        public IndexModel(ICatalogService catalogService)
        {
            _catalogService = catalogService;
        }
        [ViewData]
        public string MetaDescription { get; set; }
        [ViewData]
        public string Title { get; set; }
        public CatalogTypeViewModel CatalogTypeModel { get; set; } = new CatalogTypeViewModel();


        public async Task<IActionResult> OnGetAsync(string cat, string type, int? p)
        {
            CatalogTypeModel = await _catalogService.GetCatalogTypeItemsAsync(cat, type, p ?? 0, Constants.ITEMS_PER_PAGE);
            if (CatalogTypeModel == null)
                return NotFound();
            MetaDescription = CatalogTypeModel.MetaDescription;
            Title = CatalogTypeModel.Title;

            return Page();
        }
    }
}