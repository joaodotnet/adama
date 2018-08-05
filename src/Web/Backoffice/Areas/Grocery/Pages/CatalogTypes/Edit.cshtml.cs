using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Entities;
using Infrastructure.Data;

namespace Backoffice.Areas.Grocery.Pages.CatalogTypes
{
    public class EditModel : PageModel
    {
        private readonly Infrastructure.Data.GroceryContext _context;

        public EditModel(Infrastructure.Data.GroceryContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //check if code exists
            if (_context.CatalogTypes.Any(x => x.Code.ToUpper() == CatalogType.Code.ToUpper() && x.Id != CatalogType.Id))
            {
                ModelState.AddModelError("", $"O nome do Tipo de Produto '{CatalogType.Code}' já existe!");
                return Page();
            }

            _context.Attach(CatalogType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CatalogTypeExists(CatalogType.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool CatalogTypeExists(int id)
        {
            return _context.CatalogTypes.Any(e => e.Id == id);
        }
    }
}
