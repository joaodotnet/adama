using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DamaWeb.Pages.Basket
{
    public class ResultModel : PageModel
    {
        private readonly IOrderService _orderService;

        public ResultModel(IOrderService orderService)
        {
            _orderService = orderService;
        }
        public int OrderNumber { get; set; }
        [ViewData]
        public bool IsAddConversion { get; set; } = false;
        public async Task OnGetAsync(int id)
        {
            OrderNumber = id;

            //Adwords conversion
            var order = await _orderService.GetOrderAsync(id);

            if(order?.OrderItems?.Count > 0)
            {
                if (order.OrderItems.Any(x => x.ItemOrdered != null && x.ItemOrdered.CatalogItemId == 279))
                    IsAddConversion = true;
            }
        }
    }
}
