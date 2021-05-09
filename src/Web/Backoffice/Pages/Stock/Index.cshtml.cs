using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Backoffice.Pages.Stock
{
    public class IndexModel : PageModel
    {
        private readonly DamaContext _db;

        [FromQuery]
        [Display(Name = "Categoria")]
        public int? CatalogCategoryId { get; set; }

        [FromQuery]
        [Display(Name = "Tipo de Produto")]
        public int? CatalogTypeId { get; set; }

        [FromQuery]
        [Display(Name = "Illustração")]
        public int? CatalogIllustrationId { get; set; }

        [BindProperty]
        public IList<StockViewModel> Stocks { get; set; } = new List<StockViewModel>();

        public IndexModel(DamaContext context)
        {
            _db = context;
        }
        public async Task OnGetAsync()
        {
            PopulateList();

            var products = await _db.CatalogItems
                .Include(x => x.CatalogCategories)
                .ThenInclude(cc => cc.Category)
                .Include(x => x.CatalogAttributes)
                .Include(x => x.CatalogType)
                .Include(x => x.CatalogIllustration)
                .ThenInclude(i => i.IllustrationType)
                .Where(x => (!CatalogCategoryId.HasValue || (CatalogCategoryId.HasValue && x.CatalogCategories.All(c => c.CategoryId == CatalogCategoryId.Value))) &&
                (!CatalogTypeId.HasValue || (CatalogTypeId.HasValue && CatalogTypeId.Value == x.CatalogTypeId)) &&
                (!CatalogIllustrationId.HasValue || (CatalogIllustrationId.HasValue && CatalogIllustrationId.Value == x.CatalogIllustrationId)))
                .ToListAsync();

            foreach (var item in products)
            {
                if (item.CatalogAttributes.Count > 0)
                {
                    foreach (var attr in item.CatalogAttributes)
                    {
                        Stocks.Add(new StockViewModel
                        {
                            ProductName = item.Name,
                            CatalogItemId = item.Id,
                            CatalogAttributeId = attr.Id,
                            Category = string.Join(", ", item.CatalogCategories.Select(x => x.Category.Name)),
                            CatalogType = item.CatalogType.Name,
                            Illustration = item.CatalogIllustration.Name,
                            Attribute = attr.Name,
                            Stock = attr.Stock
                        });
                    }
                }
                else
                {
                    Stocks.Add(new StockViewModel
                    {
                        ProductName = item.Name,
                        CatalogItemId = item.Id,
                        Category = string.Join(", ", item.CatalogCategories.Select(x => x.Category.Name)),
                        CatalogType = item.CatalogType.Name,
                        Illustration = item.CatalogIllustration.Name,
                        Stock = item.Stock
                    });
                }
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            List<int> productsWithAttrs = new List<int>();
            foreach (var item in Stocks)
            {
                var prod = await _db.CatalogItems
                    .Include(x => x.CatalogAttributes)
                    .SingleOrDefaultAsync(x => x.Id == item.CatalogItemId);

                if (item.CatalogAttributeId.HasValue)
                {
                    if (!productsWithAttrs.Any(x => x == item.CatalogItemId))
                        productsWithAttrs.Add(item.CatalogItemId);
                    var attr = prod.CatalogAttributes.SingleOrDefault(x => x.Id == item.CatalogAttributeId && x.CatalogItemId == item.CatalogItemId);
                    attr.Stock = item.Stock;
                }
                else
                    prod.UpdateStock(item.Stock);
            }

            await _db.SaveChangesAsync();

            if (productsWithAttrs.Count > 0)
            {
                foreach (var item in productsWithAttrs)
                {
                    var prod = await _db.CatalogItems
                           .Include(x => x.CatalogAttributes)
                           .SingleOrDefaultAsync(x => x.Id == item);

                    prod.UpdateStock(prod.CatalogAttributes?.Sum(x => x.Stock) ?? 0);
                }

                await _db.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }

        private void PopulateList()
        {
            ViewData["CategoryList"] = new SelectList(_db.Categories, "Id", "Name");
            ViewData["CatalogTypeList"] = new SelectList(_db.CatalogTypes, "Id", "Description");
            ViewData["IllustrationList"] = new SelectList(_db.CatalogIllustrations, "Id", "Name");
        }

        public class StockViewModel
        {

            public int CatalogItemId { get; set; }
            public int? CatalogAttributeId { get; set; }
            [Display(Name = "Nome")]
            public string ProductName { get; set; }
            [Display(Name = "Categoria")]
            public string Category { get; set; }
            [Display(Name = "Tipo de Produto")]
            public string CatalogType { get; set; }
            [Display(Name = "Ilustração")]
            public string Illustration { get; set; }
            [Display(Name = "Atributo")]
            public string Attribute { get; set; }
            public int Stock { get; set; }
        }
    }
}