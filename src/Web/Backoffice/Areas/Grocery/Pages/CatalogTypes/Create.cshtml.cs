using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ApplicationCore.Entities;
using Infrastructure.Data;

namespace Backoffice.Areas.Grocery.Pages.CatalogTypes
{
    public class CreateModel : PageModel
    {
        private readonly Infrastructure.Data.GroceryContext _context;

        public CreateModel(Infrastructure.Data.GroceryContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public CatalogType CatalogType { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //check if code exists
            if (_context.CatalogTypes.Any(x => x.Code.ToUpper() == CatalogType.Code.ToUpper()))
            {
                ModelState.AddModelError("", $"O nome do Tipo de Produto '{CatalogType.Code}' já existe!");
                return Page();
            }

            _context.CatalogTypes.Add(CatalogType);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}