using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Entities;
using Infrastructure.Data;
using ApplicationCore.Interfaces;
using Backoffice.Interfaces;
using Backoffice.ViewModels;
using ApplicationCore.Entities.OrderAggregate;
using Backoffice.Extensions;

namespace Backoffice.Pages.Orders
{
    public class CustomizeOrderDetailModel : PageModel
    {
        private readonly IBackofficeService _service;
        private readonly IOrderService _orderService;

        public CustomizeOrderDetailModel(IBackofficeService service, IOrderService orderService)
        {
            _service = service;
            _orderService = orderService;
        }

        [BindProperty]
        public CustomizeOrderViewModel CustomizeOrder { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CustomizeOrder = await _service.GetCustomizeOrderAsync(id.Value);

            if (CustomizeOrder == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                await _orderService.UpdateOrderState(CustomizeOrder.Id, CustomizeOrder.OrderState, true);
                StatusMessage = $"O estado da encomenda personalizada #{CustomizeOrder.Id} foi alterada para {EnumHelper<OrderStateType>.GetDisplayValue(CustomizeOrder.OrderState)}";
                return RedirectToPage(new { id = CustomizeOrder.Id });
            }
            return Page();
        }

    }
}
