using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DamaWeb.Interfaces;
using DamaWeb.ViewModels;
using ApplicationCore;

namespace DamaWeb.Pages.Search
{
    public class IndexModel : PageModel
    {
        private readonly ICatalogService _service;

        public IndexModel(ICatalogService catalogService)
        {
            _service = catalogService;
        }

        [BindProperty]
        public string SearchFor { get; set; }

        public CatalogIndexViewModel CatalogModel { get; set; } = new CatalogIndexViewModel();

        public async Task<IActionResult> OnGetAsync(CatalogIndexViewModel catalogModel,string q, int? p)
        {
            if(string.IsNullOrEmpty(q))
                return RedirectToPage("/Index");
            SearchFor = q;
            var type = catalogModel.TypesFilterApplied;
            var illustration = CatalogModel.IllustrationFilterApplied;
            CatalogModel = await _service.GetCatalogItemsBySearch(p ?? 0, Constants.ITEMS_PER_PAGE, SearchFor, HttpContext.Request.Query["oa"]);
            CatalogModel.TypesFilterApplied = type;
            CatalogModel.IllustrationFilterApplied = illustration;
            ViewData["SearchFor"] = SearchFor;
            ViewData["TypeId"] = CatalogModel.TypesFilterApplied;
            ViewData["IllustrationId"] = CatalogModel.IllustrationFilterApplied;
            return Page();
        }

    }
}
