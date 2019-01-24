using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using System.Collections.Generic;
using System.Linq;
using System;
using System.ComponentModel.DataAnnotations;
using SalesWeb.Extensions;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore;
using Microsoft.Extensions.Options;
using ApplicationCore.Entities.OrderAggregate;

namespace SalesWeb.Pages.Order
{
    public class IndexModel : PageModel
    {
        private readonly IAsyncRepository<ApplicationCore.Entities.OrderAggregate.Order> _repository;

        public IndexModel(IAsyncRepository<ApplicationCore.Entities.OrderAggregate.Order> repository)
        {
            _repository = repository;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public List<OrderSummary> Orders { get; set; } = new List<OrderSummary>();

        public class OrderSummary
        {
            public int OrderNumber { get; set; }
            [Display(Name = "Data"), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy - HH:mm}")]
            public DateTimeOffset OrderDate { get; set; }
            public decimal Total { get; set; }
            public string Status { get; set; }
            public OrderStateType StatusType { get; set; }
            public string InvoiceNr { get; set; }
        }

        public async Task OnGet()
        {
            var orders = await _repository.ListAsync(new GroceryOrdersSpecification());

            Orders = orders
                .Select(o => new OrderSummary()
                {
                    OrderDate = o.OrderDate,
                    OrderNumber = o.Id,
                    InvoiceNr = o.SalesInvoiceNumber,
                    Status = EnumHelper<OrderStateType>.GetDisplayValue(o.OrderState),
                    StatusType = o.OrderState,
                    Total = o.Total()
                })
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }        
    }
}
