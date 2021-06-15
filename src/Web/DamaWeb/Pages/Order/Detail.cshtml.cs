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
        private readonly IRepository<ApplicationCore.Entities.OrderAggregate.Order> _orderRepository;
        private readonly IOrderService _orderService;

        public DetailModel(IRepository<ApplicationCore.Entities.OrderAggregate.Order> orderRepository, IOrderService orderService)
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
            public decimal? Total { get; set; }
            public string Status { get; set; }
            public decimal ShippingCost { get; set; }
            public decimal? Discount { get; set; }
            

            public Address ShippingAddress { get; set; }

            public Address BillingAddress { get; set; }

            public List<OrderItemViewModel> OrderItems { get; set; } = new List<OrderItemViewModel>();
            public int? TaxNumber { get; set; }
        }

        public class OrderItemViewModel
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal Discount { get; set; }
            public int Units { get; set; }
            public string PictureUrl { get; set; }
            public string CustomizeName { get; set; }
            public List<OrderItemDetailViewModel> Attributes { get; set; } = new List<OrderItemDetailViewModel>();
            [Display(Name = "Descrição")]
            public string CustomizeDescription { get; set; }
            [Display(Name = "Texto/Frase")]
            public string CustomizeText { get; set; }
            [Display(Name = "Cores")]
            public string CustomizeColors { get; set; }
            public bool IsCustomize { get; set; }
            public string DisplayProductName
            {
                get
                {
                    return !IsCustomize ? ProductName : $"Personalização: {ProductName}";
                }
            }

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
                OrderItems = await GetOrderItems(order.OrderItems),
                TaxNumber = order.TaxNumber,
                OrderNumber = order.Id,
                ShippingAddress = order.ShipToAddress,
                BillingAddress = order.BillingToAddress,
                Status = EnumHelper<OrderStateType>.GetDisplayValue(order.OrderState),
                ShippingCost = order.ShippingCost,
                Discount = order.Discount,
                Total = order.OrderItems.Any(x => x.UnitPrice == 0) ? default(decimal?) : order.Total()
            };

            return Page();
        }

        private async Task<List<OrderItemViewModel>> GetOrderItems(IReadOnlyCollection<OrderItem> orderItems)
        {
            var items = new List<OrderItemViewModel>();
            foreach (var oi in orderItems)
            {
                string uri, name, description = string.Empty, text = string.Empty, colors = string.Empty;
                int id;
                bool isCustomize = false;
                if (oi.CustomizeItem?.CatalogTypeId.HasValue == true)
                {
                    isCustomize = true;
                    id = oi.CustomizeItem.CatalogTypeId.Value;
                    uri = oi.CustomizeItem.PictureUri;
                    name = oi.CustomizeItem.ProductName;
                    description = oi.CustomizeItem.Description;
                    text = oi.CustomizeItem.Name;
                    colors = oi.CustomizeItem.Colors;
                }
                else
                {
                    id = oi.ItemOrdered.CatalogItemId;
                    uri = oi.ItemOrdered.PictureUri;
                    name = oi.ItemOrdered.ProductName;
                }
                OrderItemViewModel item = new OrderItemViewModel
                {
                    Discount = 0,
                    PictureUrl = uri,
                    ProductId = id,
                    ProductName = name,
                    CustomizeName = oi.CustomizeName,
                    UnitPrice = oi.UnitPrice,
                    Units = oi.Units,
                    Attributes = (await _orderService.GetOrderAttributesAsync(oi.ItemOrdered.CatalogItemId, oi.CatalogAttribute1, oi.CatalogAttribute2, oi.CatalogAttribute3))?
                    .Select(x => new OrderItemDetailViewModel
                    {
                        Type = x.Type,
                        AttributeName = x.Name
                    })
                    .ToList(),
                    IsCustomize = isCustomize,
                    CustomizeDescription = description,
                    CustomizeText =
                    text,
                    CustomizeColors = colors
                };
                items.Add(item);
            }
            return items;
        }
    }    
}
