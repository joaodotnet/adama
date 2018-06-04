using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ApplicationCore.Entities.OrderAggregate;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Dama.API.ViewModel;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Dama.API.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IBasketRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(IOrderService orderService, IBasketRepository basketRepository, UserManager<ApplicationUser> userManager)
        {
            _orderService = orderService;
            _repository = basketRepository;
            _userManager = userManager;
        }

        [Route("all/{userId}")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Order>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetOrders(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var orders = await _orderService.GetOrdersAsync(user.Email);

            return Ok(orders);
        }

        [Route("{id}")]
        [HttpGet]
        [ProducesResponseType(typeof(Order), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await _orderService.GetOrderAsync(id);

            return Ok(order);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Post([FromBody]OrderViewModel model)
        {
            //Get Basket
            var basketSpec = new BasketWithItemsSpecification(model.BuyerId);
            var basket = (await _repository.ListAsync(basketSpec)).LastOrDefault();
            if (basket == null)
            {
                return NotFound();
            }

            //Create Address
            Address address = new Address(null, null, model.ShippingStreet, model.ShippingCity, model.ShippingCountry, model.ShippingZipCode);
            Address billAddress = new Address();
            var order = await _orderService.CreateOrderAsync(basket.Id,null,address, billAddress, true,0);

            //Update to Submitted
            await _orderService.UpdateOrderState(order.Id, OrderStateType.SUBMITTED);
            return Ok();
        }
    }
}