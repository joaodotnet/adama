using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Entities;
using Infrastructure.Data;

namespace Backoffice.Pages.Products.References
{
    public class DeleteModel : PageModel
    {
        private readonly Infrastructure.Data.DamaContext _context;

        public DeleteModel(Infrastructure.Data.DamaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public CatalogReference CatalogReference { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CatalogReference = await _context.CatalogReferences
                .Include(c => c.CatalogItem)
                .Include(c => c.ReferenceCatalogItem).FirstOrDefaultAsync(m => m.Id == id);

            if (CatalogReference == null)
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

            CatalogReference = await _context.CatalogReferences.FindAsync(id);

            if (CatalogReference != null)
            {
                _context.CatalogReferences.Remove(CatalogReference);

                //check other reference
                var oldReference = await _context.CatalogReferences
                    .SingleOrDefaultAsync(x =>
                    x.CatalogItemId == CatalogReference.ReferenceCatalogItemId &&
                    x.ReferenceCatalogItemId == CatalogReference.CatalogItemId);
                if(oldReference != null)
                    _context.CatalogReferences.Remove(oldReference);

                await _context.SaveChangesAsync();
            }

            return RedirectToPage("/Products/Edit", new { id = CatalogReference.CatalogItemId });
        }
    }
}
