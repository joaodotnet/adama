using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using Backoffice.Interfaces;
using Backoffice.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Backoffice.Pages.Orders
{
    public class DetailsModel : PageModel
    {
        private readonly IBackofficeService _service;
        private readonly IOrderService _orderService;

        public DetailsModel(IBackofficeService service, IOrderService orderService)
        {
            _service = service;
            _orderService = orderService;
        }

        public OrderViewModel OrderModel { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            OrderModel = await _service.GetOrder(id);
            if (OrderModel == null)
                return NotFound();
            return Page();
        }

        public async Task OnPostAsync()
        {
            if(ModelState.IsValid)
            {
                await _orderService.UpdateOrderState(OrderModel.Id, OrderModel.OrderState);
            }
        }
    }
}