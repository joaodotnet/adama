using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Backoffice.Areas.Grocery.Pages.Stock
{
    public class IndexModel : PageModel
    {
        private readonly GroceryContext _db;

        [FromQuery]
        [Display(Name = "Categoria")]
        public int? CatalogCategoryId { get; set; }

        [FromQuery]
        [Display(Name = "Tipo de Produto")]
        public int? CatalogTypeId { get; set; }

        [BindProperty]
        public IList<StockViewModel> Stocks { get; set; } = new List<StockViewModel>();

        public IndexModel(GroceryContext context)
        {
            _db = context;
        }
        public async Task OnGetAsync()
        {
            PopulateList();

            var products = await _db.CatalogItems
                .Include(x => x.CatalogCategories)
                .ThenInclude(cc => cc.Category)
                .Include(x => x.CatalogType)             
                .Where(x => (!CatalogCategoryId.HasValue || (CatalogCategoryId.HasValue && x.CatalogCategories.All(c => c.CategoryId == CatalogCategoryId.Value))) &&
                (!CatalogTypeId.HasValue || (CatalogTypeId.HasValue && CatalogTypeId.Value == x.CatalogTypeId)))               
                .ToListAsync();

            Stocks = products.Select(x => new StockViewModel
            {
                ProductName = x.Name,
                CatalogItemId = x.Id,
                Category = string.Join(", ", x.CatalogCategories.Select(cc => cc.Category.Name)),
                CatalogType = x.CatalogType.Name,
                Stock = x.Stock
            }).ToList();            
        }

        public async Task<IActionResult> OnPostAsync()
        {
            List<int> productsWithAttrs = new List<int>();
            foreach (var item in Stocks)
            {
                var prod = await _db.CatalogItems
                    .Include(x => x.CatalogAttributes)
                    .SingleOrDefaultAsync(x => x.Id == item.CatalogItemId);

                prod.Stock = item.Stock;
            }

            await _db.SaveChangesAsync();           

            return RedirectToPage("./Index");
        }

        private void PopulateList()
        {
            ViewData["CategoryList"] = new SelectList(_db.Categories, "Id", "Name");
            ViewData["CatalogTypeList"] = new SelectList(_db.CatalogTypes, "Id", "Description");
        }

        public class StockViewModel
        {
            
            public int CatalogItemId { get; set; }
            [Display(Name = "Nome")]
            public string ProductName { get; set; }
            [Display(Name = "Categoria")]
            public string Category { get; set; }
            [Display(Name = "Tipo de Produto")]
            public string CatalogType { get; set; }
            public int Stock { get; set; }
        }
    }
}