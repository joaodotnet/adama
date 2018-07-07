using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ApplicationCore.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Backoffice.Pages.Products.References
{
    public class CreateModel : PageModel
    {
        private readonly Infrastructure.Data.DamaContext _context;

        public CreateModel(Infrastructure.Data.DamaContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            //ViewData["CatalogItemId"] = new SelectList(_context.CatalogItems, "Id", "Name");
            ViewData["ReferenceCatalogItemId"] = new SelectList(_context.CatalogItems, "Id", "Name");

            var prod = await _context.CatalogItems.FindAsync(id);
            if (prod == null)
                return NotFound();

            CatalogReference.CatalogItemId = id;
            CatalogReference.CatalogItem = prod;

            return Page();
        }

        [BindProperty]
        public CatalogReference CatalogReference { get; set; } = new CatalogReference();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.CatalogReferences.Add(CatalogReference);
            await _context.SaveChangesAsync();

            //Check if exists the other reference
            var referenceB = await _context.CatalogReferences
                .SingleOrDefaultAsync(x => 
                x.CatalogItemId == CatalogReference.ReferenceCatalogItemId && 
                x.ReferenceCatalogItemId == CatalogReference.CatalogItemId);
            if(referenceB == null)
            {
                _context.CatalogReferences.Add(new CatalogReference
                {
                    CatalogItemId = CatalogReference.ReferenceCatalogItemId,
                    ReferenceCatalogItemId = CatalogReference.CatalogItemId
                });
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("/Products/Edit", new { id = CatalogReference.CatalogItemId });
        }
    }
}