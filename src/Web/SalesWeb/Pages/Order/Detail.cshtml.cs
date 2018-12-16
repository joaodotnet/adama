using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ApplicationCore.Interfaces;
using System.Linq;
using ApplicationCore.Entities.OrderAggregate;
using SalesWeb.Extensions;
using ApplicationCore.Specifications;
using Microsoft.AspNetCore.Mvc;
using SalesWeb.ViewModels;
using ApplicationCore.Entities;

namespace SalesWeb.Pages.Order
{
    public partial class DetailModel : PageModel
    {
        private readonly IRepository<ApplicationCore.Entities.OrderAggregate.Order> _repository;
        private readonly IRepository<Country> _countryRepository;

        public DetailModel(IRepository<ApplicationCore.Entities.OrderAggregate.Order> repository,
            IRepository<Country> countryRepository)
        {
            _repository = repository;
            _countryRepository = countryRepository;
        }

        [BindProperty]
        public OrderViewModel OrderDetails { get; set; } = new OrderViewModel();

        public IActionResult OnGet(int orderId)
        {
            var order = _repository.GetSingleBySpec(new GroceryOrdersSpecification(orderId));
            if (order == null)
                return NotFound();

            string countryName = "Portugal";
            int.TryParse(order.BillingToAddress.Country, out int countryCode);
            if(countryCode != 0)
            {
                var country = _countryRepository.GetById(countryCode);
                if (country != null)
                    countryName = country.Name;
            }
            
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
                CountryName = countryName,
                Status = EnumHelper<OrderStateType>.GetDisplayValue(order.OrderState),
                CustomerEmail = order.CustomerEmail,
                Total = order.Total()
            };

            return Page();
        }
    }    
}
