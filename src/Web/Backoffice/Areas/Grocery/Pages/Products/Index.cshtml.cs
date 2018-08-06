using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Entities;
using Infrastructure.Data;

namespace Backoffice.Areas.Grocery.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly Infrastructure.Data.GroceryContext _context;

        public IndexModel(Infrastructure.Data.GroceryContext context)
        {
            _context = context;
        }

        public IList<CatalogItem> CatalogItem { get;set; }

        public async Task OnGetAsync()
        {
            CatalogItem = await _context.CatalogItems
                .Include(c => c.CatalogType)
                .Include(c => c.CatalogCategories)
                    .ThenInclude(cc => cc.Category)
                .ToListAsync();
        }
    }
}
