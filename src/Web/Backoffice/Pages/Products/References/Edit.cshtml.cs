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

namespace Backoffice.Pages.Products.References
{
    public class EditModel : PageModel
    {
        private readonly Infrastructure.Data.DamaContext _context;

        public EditModel(Infrastructure.Data.DamaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public CatalogReference CatalogReference { get; set; }

        [BindProperty]
        public int OriginalReference { get; set; }
        [BindProperty]
        public string OriginalLabelReference { get; set; }

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
            OriginalReference = CatalogReference.ReferenceCatalogItemId;
            OriginalLabelReference = CatalogReference.LabelDescription;


            ViewData["CatalogItemId"] = new SelectList(_context.CatalogItems, "Id", "Name");
            ViewData["ReferenceCatalogItemId"] = new SelectList(_context.CatalogItems, "Id", "Name");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(CatalogReference).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CatalogReferenceExists(CatalogReference.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            //Check Reference Product

            if (OriginalReference != CatalogReference.ReferenceCatalogItemId)
            {
                //Delete reference
                var oldReference = await _context.CatalogReferences
                .SingleOrDefaultAsync(x =>
                    x.CatalogItemId == OriginalReference &&
                    x.ReferenceCatalogItemId == CatalogReference.CatalogItemId);
                _context.CatalogReferences.Remove(oldReference);

                //Create new reference
                var newReference = await _context.CatalogReferences
                .SingleOrDefaultAsync(x =>
                x.CatalogItemId == CatalogReference.ReferenceCatalogItemId &&
                x.ReferenceCatalogItemId == CatalogReference.CatalogItemId);
                if (newReference == null)
                {
                    _context.CatalogReferences.Add(new CatalogReference
                    {
                        LabelDescription = CatalogReference.LabelDescription,
                        CatalogItemId = CatalogReference.ReferenceCatalogItemId,
                        ReferenceCatalogItemId = CatalogReference.CatalogItemId
                    });
                }
                await _context.SaveChangesAsync();
            }
            else if(!OriginalLabelReference.Equals(CatalogReference.LabelDescription))
            {
                var catalogReference = await _context.CatalogReferences
                                .SingleOrDefaultAsync(x =>
                                    x.CatalogItemId == OriginalReference &&
                                    x.ReferenceCatalogItemId == CatalogReference.CatalogItemId);
                catalogReference.LabelDescription = CatalogReference.LabelDescription;
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("/Products/Edit", new { id = CatalogReference.CatalogItemId });
        }

        private bool CatalogReferenceExists(int id)
        {
            return _context.CatalogReferences.Any(e => e.Id == id);
        }
    }
}
