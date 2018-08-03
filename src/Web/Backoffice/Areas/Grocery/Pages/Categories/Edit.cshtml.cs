using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Entities;

namespace Backoffice.Areas.Grocery.Pages.Categories
{
    public class EditModel : PageModel
    {
        private readonly Infrastructure.Data.GroceryContext _context;

        public EditModel(Infrastructure.Data.GroceryContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Category Category { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Category = await _context.Categories
                .Include(c => c.Parent).FirstOrDefaultAsync(m => m.Id == id);

            if (Category == null)
            {
                return NotFound();
            }
            PopulateList();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            PopulateList();
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //check if name exists
            if (_context.Categories.Any(x => x.Name.ToUpper() == Category.Name.ToUpper()))
            {
                ModelState.AddModelError("", $"O nome da Categoria '{Category.Name}' já existe!");
                return Page();
            }

            if (Category.ParentId == 0)
                Category.ParentId = null;

            _context.Attach(Category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(Category.Id))
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

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
        private void PopulateList()
        {
            List<Category> categories = new List<Category> { new Category { Name = "", ParentId = null } };
            if (_context.Categories.Any())
                categories.AddRange(_context.Categories.AsNoTracking().ToList());
            ViewData["ParentId"] = new SelectList(categories, "Id", "Name");
        }

    }
}
