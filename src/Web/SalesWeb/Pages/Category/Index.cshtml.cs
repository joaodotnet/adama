using System;
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

        public NewCategoryModel SalesCategoryModel { get; set; } = new NewCategoryModel();

        [TempData]
        public string StatusMessage { get; set; }

        public class NewCategoryModel
        {
            public List<NewCategoryTypes> Types { get; set; } = new List<NewCategoryTypes>();
        }
        public class NewCategoryTypes
        {
            public string Name { get; set; }
            public List<CatalogItemViewModel> CatalogItems { get; set; } = new List<CatalogItemViewModel>();
        }

        public async Task<IActionResult> OnGetAsync(string id, int? p)
        {
            var cat = await _catalogService.GetCategory(id); 
            if (!cat.HasValue)
                return NotFound();
            else
                CategoryModel.CategoryName = cat.Value.Item2;

            CategoryModel.CatalogModel = await _catalogService.GetCategoryCatalogItems(cat.Value.Item1, 0, null);
            //CategoryModel.CatalogTypes = CategoryModel.CatalogModel.CatalogItems.Select(x => (x.CatalogTypeCode,x.CatalogTypeName)).Distinct().ToList();
            CategoryModel.CategoryUrlName = id.ToLower();

            var groupByType = CategoryModel.CatalogModel.CatalogItems.GroupBy(x => x.CatalogTypeName);
            foreach (var item in groupByType)
            {
                SalesCategoryModel.Types.Add(new NewCategoryTypes
                {
                    Name = item.Key,
                    CatalogItems = item.ToList()
                });
            }

            return Page();
        }
    }

    
}