using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Entities;
using Infrastructure.Data;
using Backoffice.ViewModels;
using AutoMapper;

namespace Backoffice.Pages.ShippingPrice
{
    public class DetailsModel : PageModel
    {
        private readonly Infrastructure.Data.DamaContext _context;
        private readonly IMapper _mapper;

        public DetailsModel(Infrastructure.Data.DamaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

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
    }
}
