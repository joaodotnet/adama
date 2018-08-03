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

namespace Backoffice.Areas.Grocery.Pages.Categories
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
            PopulateList();
            return Page();
        }        

        [BindProperty]
        public Category Category { get; set; }

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

            _context.Categories.Add(Category);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
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