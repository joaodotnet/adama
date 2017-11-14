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
using AutoMapper;
using Backoffice.ViewModels;

namespace Backoffice.Pages.IllustrationTypes
{
    public class EditModel : PageModel
    {
        private readonly DamaContext _context;
        private readonly IMapper _mapper;

        public EditModel(Infrastructure.Data.DamaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //check if code exists
            if (_context.IllustrationTypes.Any(x => x.Code.ToUpper() == IllustrationType.Code.ToUpper() && x.Id != IllustrationType.Id))
            {
                ModelState.AddModelError("", $"O código do Tipo de Ilustração '{IllustrationType.Code}' já existe!");
                return Page();
            }

            _context.Attach(_mapper.Map<IllustrationType>(IllustrationType)).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                
            }

            return RedirectToPage("./Index");
        }
    }
}
