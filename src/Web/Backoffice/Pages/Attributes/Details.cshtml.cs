using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Entities;
using Infrastructure.Data;

namespace Backoffice.Pages.Attributes
{
    public class DetailsModel : PageModel
    {
        private readonly Infrastructure.Data.DamaContext _context;

        public DetailsModel(Infrastructure.Data.DamaContext context)
        {
            _context = context;
        }

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
    }
}
