using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore;
using ApplicationCore.Interfaces;
using AutoMapper;
using Backoffice.Interfaces;
using Backoffice.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Backoffice.Areas.Grocery.Pages.Sales
{
    public class IndexModel : PageModel
    {
        private readonly Infrastructure.Data.GroceryContext _context;
        private readonly IMapper _mapper;

        public IndexModel(
            Infrastructure.Data.GroceryContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public List<OrderViewModel> OrdersModel { get; set; } = new List<OrderViewModel>();
        public async Task<IActionResult> OnGetAsync()
        {
            var orders  = await _context.Orders
                .Include(x => x.OrderItems)
                .OrderByDescending(x => x.Id)
                .ToListAsync();
            OrdersModel = _mapper.Map<List<OrderViewModel>>(orders);
            return Page();
        }
    }
}