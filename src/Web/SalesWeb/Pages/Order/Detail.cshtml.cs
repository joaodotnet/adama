using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ApplicationCore.Interfaces;
using System.Linq;
using ApplicationCore.Entities.OrderAggregate;
using SalesWeb.Extensions;
using ApplicationCore.Specifications;
using Microsoft.AspNetCore.Mvc;
using SalesWeb.ViewModels;

namespace SalesWeb.Pages.Order
{
    public partial class DetailModel : PageModel
    {
        private readonly IRepository<ApplicationCore.Entities.OrderAggregate.Order> _repository;

        public DetailModel(IRepository<ApplicationCore.Entities.OrderAggregate.Order> repository)
        {
            _repository = repository;
        }

        [BindProperty]
        public OrderViewModel OrderDetails { get; set; } = new OrderViewModel();

        public IActionResult OnGet(int orderId)
        {
            var order = _repository.GetSingleBySpec(new GroceryOrdersSpecification(orderId));
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
                    CustomizeName = oi.CustomizeName,
                    UnitPrice = oi.UnitPrice,
                    Units = oi.Units
                    //Attributes = _orderService.GetOrderAttributes(oi.ItemOrdered.CatalogItemId, oi.CatalogAttribute1, oi.CatalogAttribute2, oi.CatalogAttribute3)
                    //.Select(x => new OrderItemDetailViewModel
                    //{
                    //    Type = x.Type,
                    //    AttributeName = x.Name
                    //})
                    //.ToList()
                }).ToList(),

                OrderNumber = order.Id,
                InvoiceNr = order.SalesInvoiceNumber,
                InvoiceId = order.SalesInvoiceId.Value,
                BillingAddress = order.BillingToAddress,
                Status = EnumHelper<OrderStateType>.GetDisplayValue(order.OrderState),
                CustomerEmail = order.CustomerEmail,
                Total = order.Total()
            };

            return Page();
        }
    }    
}
