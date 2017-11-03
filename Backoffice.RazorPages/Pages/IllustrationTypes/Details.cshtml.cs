using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Entities;
using Infrastructure.Data;
using AutoMapper;
using Backoffice.RazorPages.ViewModels;

namespace Backoffice.RazorPages.Pages.IllustrationTypes
{
    public class DetailsModel : PageModel
    {
        private readonly DamaContext _context;
        private readonly IMapper _mapper;

        public DetailsModel(DamaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IllustrationTypeViewModel IllustrationType { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            IllustrationType = _mapper.Map<IllustrationTypeViewModel>(await _context.IllustrationTypes.SingleOrDefaultAsync(m => m.Id == id));

            if (IllustrationType == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
