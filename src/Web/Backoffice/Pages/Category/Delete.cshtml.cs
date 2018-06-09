using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Backoffice.ViewModels;
using AutoMapper;

namespace Backoffice.Pages.Category
{
    public class DeleteModel : PageModel
    {
        private readonly Infrastructure.Data.DamaContext _context;
        protected readonly IMapper _mapper;

        public DeleteModel(Infrastructure.Data.DamaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [BindProperty]
        public CategoryViewModel Category { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var catInDB = await _context.Categories.SingleOrDefaultAsync(m => m.Id == id);

            if (catInDB == null)
            {
                return NotFound();
            }

            Category = _mapper.Map<CategoryViewModel>(catInDB);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var catinDb = await _context.Categories.FindAsync(id);

            if (catinDb != null)
            {
                _context.Categories.Remove(catinDb);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
