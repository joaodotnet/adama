using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using System.Collections.Generic;
using System.Linq;
using System;
using System.ComponentModel.DataAnnotations;
using Web.Extensions;
using ApplicationCore.Entities.OrderAggregate;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore;
using Microsoft.Extensions.Options;

namespace Web.Pages.Order
{
    public class IndexModel : PageModel
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderService _orderService;
        private readonly IEmailSender _emailSender;
        private readonly CatalogSettings _settings;

        public IndexModel(IOrderRepository orderRepository, IOrderService orderService, IEmailSender emailSender, IOptions<CatalogSettings> settings)
        {
            _orderRepository = orderRepository;
            _orderService = orderService;
            _emailSender = emailSender;
            _settings = settings.Value;
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
        }

        public async Task OnGet()
        {
            var orders = await _orderRepository.ListAsync(new CustomerOrdersWithItemsSpecification(User.Identity.Name));

            Orders = orders
                .Select(o => new OrderSummary()
                {
                    OrderDate = o.OrderDate,
                    OrderNumber = o.Id,
                    Status = EnumHelper<OrderStateType>.GetDisplayValue(o.OrderState),
                    StatusType = o.OrderState,
                    Total = o.Total()
                })
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }
        public async Task<IActionResult> OnGetCancelOrder(int number)
        {
            //Check order number is belong to user
            var order = await _orderRepository.GetByIdAsync(number);
            if(order != null && order.BuyerId.ToLower() == User.Identity.Name.ToLower())
            {
                await _orderService.UpdateOrderState(order.Id, OrderStateType.CANCELED);
                StatusMessage = $"A encomenda #{number} foi cancelada com sucesso!";
                var body = $"A encomenda <a href='http://backoffice.damanojornal.com/Orders/Details?id={order.Id}'> #{order.Id}</a> foi cancelada!";
                await _emailSender.SendEmailAsync(_settings.ToEmails, $"A encomenda #{order.Id} foi cancelada!", body);
            }
            else
                StatusMessage = $"Erro: Encomenda {number} não existe!";
            return RedirectToPage();
        }
    }
}
