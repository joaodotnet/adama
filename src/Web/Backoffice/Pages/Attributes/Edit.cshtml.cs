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

namespace Backoffice.Pages.Attributes
{
    public class EditModel : PageModel
    {
        private readonly Infrastructure.Data.DamaContext _context;

        public EditModel(Infrastructure.Data.DamaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ApplicationCore.Entities.Attribute Attribute { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Attribute = await _context.Attributes.FirstOrDefaultAsync(m => m.Id == id);

            if (Attribute == null)
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

            _context.Attach(Attribute).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AttributeExists(Attribute.Id))
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

        private bool AttributeExists(int id)
        {
            return _context.Attributes.Any(e => e.Id == id);
        }
    }
}
