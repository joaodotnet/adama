using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore;
using ApplicationCore.Interfaces;
using Backoffice.Interfaces;
using Backoffice.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Backoffice.Pages.Orders
{
    public class IndexModel : PageModel
    {
        private readonly IBackofficeService _service;

        public IndexModel(IBackofficeService service)
        {
            _service = service;
        }
        public List<OrderViewModel> OrdersModel { get; set; } = new List<OrderViewModel>();
        public async Task<IActionResult> OnGetAsync()
        {          
            OrdersModel = await _service.GetOrders();
            return Page();
        }
    }
}