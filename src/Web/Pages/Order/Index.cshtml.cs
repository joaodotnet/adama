using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using System.Collections.Generic;
using System.Linq;
using System;
using System.ComponentModel.DataAnnotations;

namespace Web.Pages.Order
{
    public class IndexModel : PageModel
    {
        private readonly IOrderRepository _orderRepository;

        public IndexModel(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public List<OrderSummary> Orders { get; set; } = new List<OrderSummary>();

        public class OrderSummary
        {
            public int OrderNumber { get; set; }
            [Display(Name = "Data"), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy - hh:mm}")]
            public DateTimeOffset OrderDate { get; set; }
            public decimal Total { get; set; }
            public string Status { get; set; }
        }

        public async Task OnGet()
        {
            var orders = await _orderRepository.ListAsync(new CustomerOrdersWithItemsSpecification(User.Identity.Name));

            Orders = orders
                .Select(o => new OrderSummary()
                {
                    OrderDate = o.OrderDate,
                    OrderNumber = o.Id,
                    Status = "Pending",
                    Total = o.Total()

                })
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }
    }
}
