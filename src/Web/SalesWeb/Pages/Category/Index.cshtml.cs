﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalesWeb.Interfaces;
using SalesWeb.ViewModels;

namespace SalesWeb.Pages.Category
{
    public class IndexModel : PageModel
    {
        private readonly ICatalogService _catalogService;
        public IndexModel(ICatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        public CategoryViewModel CategoryModel { get; set; } = new CategoryViewModel();

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string id, int? p)
        {
            var cat = await _catalogService.GetCategory(id); 
            if (!cat.HasValue)
                return NotFound();
            else
                CategoryModel.CategoryName = cat.Value.Item2;

            CategoryModel.CatalogModel = await _catalogService.GetCategoryCatalogItems(cat.Value.Item1, p ?? 0, Constants.ITEMS_PER_PAGE);
            //CategoryModel.CatalogTypes = CategoryModel.CatalogModel.CatalogItems.Select(x => (x.CatalogTypeCode,x.CatalogTypeName)).Distinct().ToList();
            CategoryModel.CategoryUrlName = id.ToLower();

            return Page();
        }
    }
}