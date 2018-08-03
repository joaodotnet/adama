using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Entities;
using Infrastructure.Data;

namespace Backoffice.Areas.Grocery.Pages.CatalogTypes
{
    public class IndexModel : PageModel
    {
        private readonly Infrastructure.Data.GroceryContext _context;

        public IndexModel(Infrastructure.Data.GroceryContext context)
        {
            _context = context;
        }

        public IList<CatalogType> CatalogType { get;set; }

        public async Task OnGetAsync()
        {
            CatalogType = await _context.CatalogTypes.ToListAsync();
        }
    }
}
