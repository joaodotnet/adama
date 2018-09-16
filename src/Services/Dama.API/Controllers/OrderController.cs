using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ApplicationCore;
using ApplicationCore.Entities.OrderAggregate;
using ApplicationCore.Interfaces;
using ApplicationCore.Services;
using ApplicationCore.Specifications;
using Dama.API.Interfaces;
using Dama.API.Services;
using Dama.API.ViewModel;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Dama.API.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/orders")]
    [Route("api/grocery/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderDamaService;
        private readonly IOrderGroceryService _orderGroceryService;
        private readonly IDamaRepository _damaRepository;
        private readonly IGroceryRepository _groceryRepository;
        //private readonly IBasketRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IInvoiceService _invoiceService;
        private readonly IEmailSender _emailSender;
        private readonly EmailSettings _settings;


        public OrderController(IOrderService orderService, 
           IOrderGroceryService orderGroceryService, 
           IDamaRepository damaRepo,
           IGroceryRepository groceryRepo, 
           UserManager<ApplicationUser> userManager,
           IInvoiceService invoiceService, 
           IEmailSender emailSender,
           IOptions<EmailSettings> options)
        {
            _orderDamaService = orderService;
            _orderGroceryService = orderGroceryService;
            _damaRepository = damaRepo;
            _groceryRepository = groceryRepo;
            _userManager = userManager;
            _invoiceService = invoiceService;
            _emailSender = emailSender;
            _settings = options.Value;
        }

        private IOrderService OrderService
        {
            get
            {
                return Request.Path.Value.Contains("grocery") ? _orderGroceryService : _orderDamaService;
            }
        }

        private IDamaRepository Repository
        {
            get
            {
                return Request.Path.Value.Contains("grocery") ? _groceryRepository : _damaRepository;
            }
        }

        [Route("all/{userId}")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Order>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetOrders(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            var orders = new List<Order>();
            if (user.UserName == "sue@damanojornal.com" || user.UserName == "jue@damanojornal.com" || user.UserName == "sonia@damanojornal.com")
            {
                orders = await OrderService.GetOrdersAsync("sue@damanojornal.com");
                orders.AddRange(await OrderService.GetOrdersAsync("jue@damanojornal.com"));
                orders.AddRange(await OrderService.GetOrdersAsync("sonia@damanojornal.com"));
            }
            else
            {
                orders = await OrderService.GetOrdersAsync(user.Email);
            }



            return Ok(orders);
        }

        [Route("{id}")]
        [HttpGet]
        [ProducesResponseType(typeof(Order), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await OrderService.GetOrderAsync(id);

            return Ok(order);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Post([FromBody]OrderViewModel model)
        {
            //Get Basket
            var basketSpec = new BasketWithItemsSpecification(model.BuyerId);
            var basket = (await Repository.ListAsync(basketSpec)).LastOrDefault();
            if (basket == null)
            {
                return NotFound();
            }

            //Create Address
            Address address = new Address(null, model.ShippingStreet, model.ShippingCity, model.ShippingCountry, model.ShippingZipCode);
            Address billAddress = new Address(model.BillingName, model.BillingStreet, model.BillingCity, model.BillingCountry, model.BillingPostalCode);
            var order = await OrderService.CreateOrderAsync(basket.Id, null, model.TaxNumber, address, billAddress, true, 0, model.CustomerEmail);

            //Update to Submitted
            await OrderService.UpdateOrderState(order.Id, OrderStateType.SUBMITTED);

            //Create Invoice
            if(model.CreateInvoice)
            {
                long? invoiceId = null;
                try
                {
                    var response = await _invoiceService.RegisterInvoiceAsync(order);
                    if (response != null && response.InvoiceId.HasValue)
                    {
                        invoiceId = response.InvoiceId;
                        await OrderService.UpdateOrderInvoiceAsync(order.Id, response.InvoiceId, response.InvoiceNumber);
                        model.ResultMessage = "Fatura Gerada com sucesso";
                    }
                    else
                        model.ResultMessage = $"Erro na criação da fatura ({response.Message})";
                }
                catch (Exception)
                {
                    model.ResultMessage = "Erro na criação da Fatura (genérico)";
                }
                //Get Invoice PDF and save to Disk
                if (invoiceId.HasValue)
                {
                    var invoiceBytes = await _invoiceService.GetPDFInvoiceAsync(invoiceId.Value);
                    if(invoiceBytes != null)
                    {
                        //Send Email to client (from: info.saborcomtradicao@gmail.com)

                        var body = $"<strong>Olá!</strong><br>" +
                            $"Obrigada por comprares na Sabor com Tradição.<br>" +
                            $"Enviamos em anexo a fatura relativa à tua encomenda. <br>" +
                            "<br>Muito Obrigada.<br>" +
                            "<br>--------------------------------------------------<br>" +
                            "<br><strong>Hi!</strong><br>" +
                            "Thank you to shopping at Sabor Com Tradição in Loulé, Portugal. <br>" +
                            "We send as attach the invoice relates to your order.<br>" +                            
                            "<br>Thank you.<br>" +
                            "<br>Sabor com Tradição" +
                            "<br>http://www.saborcomtradicao.com";

                        List<(string, byte[])> files = new List<(string, byte[])>();                        
                        files.Add(($"FaturaSaborComTradicao#{order.Id}.pdf", invoiceBytes));
                        
                        await _emailSender.SendEmailAsync(
                            _settings.FromOrderEmail, 
                            !string.IsNullOrEmpty(model.CustomerEmail) ? model.CustomerEmail : _settings.CCEmails, 
                            $"Sabor com Tradição - Encomenda #{order.Id}", 
                            body, 
                            _settings.CCEmails, 
                            null, 
                            files);
                    }
                }
            }
            return Ok(model);
        }

        [Route("cancel")]
        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CancelOrder([FromBody]CancelOrderViewModel command)
        {
            await OrderService.UpdateOrderState(command.OrderNumber, OrderStateType.CANCELED);
            return Ok();

        }

        [Route("place/{placeId}")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Order>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetOrdersByPlace(int placeId)
        {
            var street = PlaceIdToStreet(placeId);
            var orders = new List<Order>();
            orders = (await OrderService
                .GetOrdersAsync("sue@damanojornal.com"))
                .Where(o => (placeId != 3 && o.ShipToAddress.Street.Equals(street)) || 
                    (placeId == 3 && o.OrderDate > DateTime.Now.AddMonths(-1)))
                .ToList();
            orders.AddRange((await OrderService
                .GetOrdersAsync("jue@damanojornal.com"))
                .Where(o => (placeId != 3 && o.ShipToAddress.Street.Equals(street)) || 
                    (placeId == 3 && o.OrderDate > DateTime.Now.AddMonths(-1)))
                .ToList());
            orders.AddRange((await OrderService
                .GetOrdersAsync("sonia@damanojornal.com"))
                .Where(o => (placeId != 3 && o.ShipToAddress.Street.Equals(street)) || 
                    (placeId == 3 && o.OrderDate > DateTime.Now.AddMonths(-1)))
                .ToList());

            if(placeId == 3)
            {
                orders.AddRange((await OrderService
                .GetOrdersAsync("rute@damanojornal.com"))
                .Where(o => o.OrderDate > DateTime.Now.AddMonths(-1))
                .ToList());
            }

            return Ok(orders);
        }

        private object PlaceIdToStreet(int placeId)
        {
            if (placeId == 1)
                return "Feira Popular de Loulé";
            if (placeId == 2)
                return "Feira da Serra de São Brás";
            return string.Empty;
        }
    }
}