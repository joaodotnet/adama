using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Dama.API.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Dama.API.Controllers
{
    [Route("api/v1/[controller]")]
    public class BasketController : Controller
    {
        private readonly IBasketRepository _repository;
        private readonly IRepository<CatalogItem> _itemRepository;
        //private readonly IIdentityService _identitySvc;
        //private readonly IEventBus _eventBus;

        public BasketController(IBasketRepository repository,
            IRepository<CatalogItem> itemRepository)
            //IIdentityService identityService,
            //IEventBus eventBus)
        {
            _repository = repository;
            _itemRepository = itemRepository;
            //_identitySvc = identityService;
            //_eventBus = eventBus;
        }

        // GET /id
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Basket), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get(string id)
        {
            var basketSpec = new BasketWithItemsSpecification(id);
            var basket = (await _repository.ListAsync(basketSpec)).LastOrDefault();
            if (basket == null)
            {
                return NotFound();
            }

            return Ok(BasketToViewModel(basket));
        }

        

        // POST /value
        [HttpPost]
        [ProducesResponseType(typeof(Basket), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Post([FromBody]BasketViewModel value)
        {
            var basket = await _repository.UpdateBasketAsync(ViewModelToBasket(value));

            return Ok(basket);
        }        

        //[Route("checkout")]
        //[HttpPost]
        //[ProducesResponseType((int)HttpStatusCode.Accepted)]
        //[ProducesResponseType((int)HttpStatusCode.BadRequest)]
        //public async Task<IActionResult> Checkout([FromBody]BasketCheckout basketCheckout, [FromHeader(Name = "x-requestid")] string requestId)
        //{
        //    var userId = _identitySvc.GetUserIdentity();
        //    basketCheckout.RequestId = (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty) ?
        //        guid : basketCheckout.RequestId;

        //    var basket = await _repository.GetBasketAsync(userId);

        //    if (basket == null)
        //    {
        //        return BadRequest();
        //    }

        //    var eventMessage = new UserCheckoutAcceptedIntegrationEvent(userId, basketCheckout.City, basketCheckout.Street,
        //        basketCheckout.State, basketCheckout.Country, basketCheckout.ZipCode, basketCheckout.CardNumber, basketCheckout.CardHolderName,
        //        basketCheckout.CardExpiration, basketCheckout.CardSecurityNumber, basketCheckout.CardTypeId, basketCheckout.Buyer, basketCheckout.RequestId, basket);

        //    // Once basket is checkout, sends an integration event to
        //    // ordering.api to convert basket to order and proceeds with
        //    // order creation process
        //    _eventBus.Publish(eventMessage);

        //    return Accepted();
        //}

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task DeleteAsync(string id)
        {
            var basketSpec = new BasketWithItemsSpecification(id);
            var basket = (await _repository.ListAsync(basketSpec)).LastOrDefault();
            await _repository.DeleteBasketAsync(basket.Id);
        }

        private BasketViewModel BasketToViewModel(Basket basket)
        {
            return new BasketViewModel
            {
                BuyerId = basket.BuyerId,
                Items = basket.Items.Select(i =>
                {
                    var itemModel = new BasketItemViewModel
                    {
                        Id = i.Id,
                        ProductId = i.CatalogItemId,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        OldUnitPrice = 0m,
                    };
                    var item = _itemRepository.GetById(i.CatalogItemId);
                    if (item != null)
                    {
                        itemModel.PictureUrl = item.PictureUri;
                        itemModel.ProductName = item.Name;
                    }
                    //itemModel.Attributes = i.Details.Select(d => new AttributeViewModel
                    //{
                    //    Name = d.CatalogAttribute.Name,
                    //    Label = EnumHelper<CatalogAttributeType>.GetDisplayValue(d.CatalogAttribute.Type)
                    //}).ToList();
                    return itemModel;
                }).ToList()
            };
        }

        private Basket ViewModelToBasket(BasketViewModel model)
        {
            var basket = new Basket
            {
                BuyerId = model.BuyerId
            };
            foreach (var item in model.Items)
            {
                basket.AddItem(item.ProductId, item.UnitPrice, item.Quantity);
            }
            return basket;
        }
    }
}
