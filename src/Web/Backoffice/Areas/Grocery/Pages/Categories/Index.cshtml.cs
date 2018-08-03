using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Entities;
using Infrastructure.Data;

namespace Backoffice.Areas.Grocery.Pages.Categories
{
    public class IndexModel : PageModel
    {
        private readonly Infrastructure.Data.GroceryContext _context;

        public IndexModel(Infrastructure.Data.GroceryContext context)
        {
            _context = context;
        }

        public IList<Category> Category { get;set; }

        public async Task OnGetAsync()
        {
            Category = await _context.Categories
                .Include(c => c.Parent).ToListAsync();
        }
    }
}
