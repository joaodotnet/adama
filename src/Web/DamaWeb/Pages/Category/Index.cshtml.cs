using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DamaWeb.Interfaces;
using DamaWeb.ViewModels;

namespace DamaWeb.Pages.Category
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

        public CategoryViewModel CategoryModel { get; set; } = new CategoryViewModel();

        public async Task<IActionResult> OnGetAsync(string id, int? p)
        {
            //var cat = await _catalogService.GetCategory(id); 
            //if (!cat.HasValue)
            //    return NotFound();

            CategoryModel = await _catalogService.GetCategoryCatalogItems(id, p ?? 0, Constants.ITEMS_PER_PAGE);
            MetaDescription = CategoryModel.MetaDescription;
            Title = CategoryModel.Title;

            return Page();
        }
    }
}