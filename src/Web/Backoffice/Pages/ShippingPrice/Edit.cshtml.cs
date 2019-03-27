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
using Backoffice.ViewModels;
using AutoMapper;

namespace Backoffice.Pages.ShippingPrice
{
    public class EditModel : PageModel
    {
        private readonly Infrastructure.Data.DamaContext _context;
        private readonly IMapper _mapper;

        public EditModel(Infrastructure.Data.DamaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [BindProperty]
        public ShippingPriceViewModel ShippingPriceWeight { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.ShippingPriceWeights.FirstOrDefaultAsync(m => m.Id == id);

            if (item == null)
            {
                return NotFound();
            }
            ShippingPriceWeight = _mapper.Map<ShippingPriceViewModel>(item);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(_mapper.Map<ShippingPriceWeight>(ShippingPriceWeight)).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShippingPriceWeightExists(ShippingPriceWeight.Id))
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

        private bool ShippingPriceWeightExists(int id)
        {
            return _context.ShippingPriceWeights.Any(e => e.Id == id);
        }
    }
}
