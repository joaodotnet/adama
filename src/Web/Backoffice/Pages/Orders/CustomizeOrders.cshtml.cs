using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Entities;
using Infrastructure.Data;
using Backoffice.Interfaces;
using Backoffice.ViewModels;

namespace Backoffice.Pages.Orders
{
    public class CustomizeOrdersModel : PageModel
    {
        private readonly IBackofficeService _service;

        public CustomizeOrdersModel(IBackofficeService service)
        {
            _service = service;
        }

        public IList<CustomizeOrderViewModel> CustomizeOrder { get; set; } = new List<CustomizeOrderViewModel>();

        public async Task OnGetAsync()
        {
            CustomizeOrder = await _service.GetCustomizeOrdersAsync();
        }
    }
}
