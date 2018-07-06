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
    public class IndexModel : PageModel
    {
        private readonly Infrastructure.Data.DamaContext _context;

        public IndexModel(Infrastructure.Data.DamaContext context)
        {
            _context = context;
        }

        public IList<ApplicationCore.Entities.Attribute> Attribute { get;set; }

        public async Task OnGetAsync()
        {
            Attribute = await _context.Attributes.ToListAsync();
        }
    }
}
