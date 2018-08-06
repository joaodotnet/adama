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
    public class DeleteModel : PageModel
    {
        private readonly Infrastructure.Data.GroceryContext _context;

        public DeleteModel(Infrastructure.Data.GroceryContext context)
        {
            _context = context;
        }

        [BindProperty]
        public CatalogItem CatalogItem { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CatalogItem = await _context.CatalogItems
                .Include(c => c.CatalogType).FirstOrDefaultAsync(m => m.Id == id);

            if (CatalogItem == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CatalogItem = await _context.CatalogItems.FindAsync(id);

            if (CatalogItem != null)
            {
                _context.CatalogItems.Remove(CatalogItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
