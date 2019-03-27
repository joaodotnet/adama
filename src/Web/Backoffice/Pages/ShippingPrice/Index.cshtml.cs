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
    public class IndexModel : PageModel
    {
        private readonly Infrastructure.Data.DamaContext _context;
        private readonly IMapper _mapper;

        public IndexModel(Infrastructure.Data.DamaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IList<ShippingPriceViewModel> ShippingPriceWeight { get;set; }

        public async Task OnGetAsync()
        {
            ShippingPriceWeight = _mapper.Map<IList<ShippingPriceViewModel>>(await _context.ShippingPriceWeights.ToListAsync());
        }
    }
}
