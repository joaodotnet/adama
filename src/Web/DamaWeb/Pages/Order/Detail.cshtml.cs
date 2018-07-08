using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ApplicationCore.Interfaces;
using System.Linq;
using System;
using ApplicationCore.Entities.OrderAggregate;
using System.Collections.Generic;
using DamaWeb.Extensions;
using ApplicationCore.Entities;
using System.ComponentModel.DataAnnotations;
using ApplicationCore.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace DamaWeb.Pages.Order
{
    public class DetailModel : PageModel
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderService _orderService;

        public DetailModel(IOrderRepository orderRepository, IOrderService orderService)
        {
            _orderRepository = orderRepository;
            this._orderService = orderService;
        }

        public OrderViewModel OrderDetails { get; set; } = new OrderViewModel();

        public class OrderViewModel
        {
            public int OrderNumber { get; set; }
            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy - HH:mm}")]
            public DateTimeOffset OrderDate { get; set; }
            public decimal Total { get; set; }
            public string Status { get; set; }
            public decimal ShippingCost { get; set; }

            public Address ShippingAddress { get; set; }

            public Address BillingAddress { get; set; }

            public List<OrderItemViewModel> OrderItems { get; set; } = new List<OrderItemViewModel>();
        }

        public class OrderItemViewModel
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal Discount { get; set; }
            public int Units { get; set; }
            public string PictureUrl { get; set; }
            public List<OrderItemDetailViewModel> Attributes { get; set; } = new List<OrderItemDetailViewModel>();
        }

        public class OrderItemDetailViewModel
        {
            public AttributeType Type { get; set; }
            public string AttributeName { get; set; }
        }


        public async Task<IActionResult> OnGet(int orderId)
        {
            var orders = await _orderRepository.ListAsync(new CustomerOrdersWithItemsSpecification(User.Identity.Name));

            //var order = await _orderRepository.GetByIdWithItemsAsync(orderId);
            var order = orders.SingleOrDefault(x => x.Id == orderId);
            if (order == null)
                return NotFound();

            OrderDetails = new OrderViewModel()
            {
                OrderDate = order.OrderDate,
                OrderItems = order.OrderItems.Select(oi => new OrderItemViewModel()
                {
                    Discount = 0,
                    PictureUrl = oi.ItemOrdered.PictureUri,
                    ProductId = oi.ItemOrdered.CatalogItemId,
                    ProductName = oi.ItemOrdered.ProductName,
                    UnitPrice = oi.UnitPrice,
                    Units = oi.Units,
                    Attributes = _orderService.GetOrderAttributes(oi.ItemOrdered.CatalogItemId, oi.CatalogAttribute1,oi.CatalogAttribute2,oi.CatalogAttribute3)
                    .Select(x => new OrderItemDetailViewModel
                    {
                        Type = x.Type,
                        AttributeName = x.Name
                    })
                    .ToList()
                }).ToList(),
                
                OrderNumber = order.Id,
                ShippingAddress = order.ShipToAddress,
                BillingAddress = order.BillingToAddress,
                Status = EnumHelper<OrderStateType>.GetDisplayValue(order.OrderState),
                ShippingCost = order.ShippingCost,
                Total = order.Total()
            };

            return Page();
        }
    }    
}
