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
    public class DetailsModel : PageModel
    {
        private readonly Infrastructure.Data.GroceryContext _context;

        public DetailsModel(Infrastructure.Data.GroceryContext context)
        {
            _context = context;
        }

        public CatalogType CatalogType { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CatalogType = await _context.CatalogTypes.FirstOrDefaultAsync(m => m.Id == id);

            if (CatalogType == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
