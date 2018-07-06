using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ApplicationCore.Entities;
using Infrastructure.Data;

namespace Backoffice.Pages.Attributes
{
    public class CreateModel : PageModel
    {
        private readonly Infrastructure.Data.DamaContext _context;

        public CreateModel(Infrastructure.Data.DamaContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public ApplicationCore.Entities.Attribute Attribute { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attributes.Add(Attribute);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}