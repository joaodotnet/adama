using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Dama.API.ViewModels;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Dama.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
//    [Authorize]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _repository;
        private readonly IRepository<CatalogItem> _itemRepository;
        private readonly IRepository<BasketItem> _basketItemRepository;
        private readonly IRepository<CatalogAttribute> _attributeRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        //private readonly IIdentityService _identitySvc;
        //private readonly IEventBus _eventBus;

        public BasketController(IBasketRepository repository,
            IRepository<CatalogItem> itemRepository,
            IRepository<BasketItem> basketItemRepository,
            UserManager<ApplicationUser> userManager,
            IRepository<CatalogAttribute> attributeRepository)            
        {
            _repository = repository;
            _itemRepository = itemRepository;
            _basketItemRepository = basketItemRepository;
            _userManager = userManager;
            _attributeRepository = attributeRepository;
        }

        // GET /id
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BasketViewModel), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var basketSpec = new BasketWithItemsSpecification(user.Email);
            var basket = (await _repository.ListAsync(basketSpec)).LastOrDefault();
            if (basket == null)
            {
                return NotFound();
            }

            return Ok(BasketToViewModel(basket));
        }

        // POST /value
        //[HttpPost]
        //[ProducesResponseType(typeof(Basket), (int)HttpStatusCode.OK)]
        //public async Task<IActionResult> Post([FromBody]BasketViewModel value)
        //{
        //    var basket = await _repository.UpdateBasketAsync(ViewModelToBasket(value));

        //    return Ok(basket);
        //}

        [Route("add")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> AddItem([FromBody]BasketViewModel value)
        {
            var basketModel = ViewModelToBasket(value);
            var basketDb = await GetOrCreateBasketForUser(value.BuyerId);
            await _repository.AddBasketItemAsync(basketDb.Id, basketModel.Items.FirstOrDefault());

            return Ok();
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
        [HttpDelete("{userId}")]
        public async Task DeleteAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var basketSpec = new BasketWithItemsSpecification(user.Email);
            var basket = (await _repository.ListAsync(basketSpec)).LastOrDefault();
            await _repository.DeleteAsync(basket);
        }

        // DELETE api/values/5
        [HttpDelete("{userId}/deleteItem/{basketItemId}")]
        public async Task DeleteAsync(string userId, int basketItemId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var basketSpec = new BasketWithItemsSpecification(user.Email);
            var basket = (await _repository.ListAsync(basketSpec)).LastOrDefault();
            if (basket.Items.Any(x => x.Id == basketItemId))
                _basketItemRepository.Delete(_basketItemRepository.GetById(basketItemId));
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

                    AddAttributeToModel(i.CatalogAttribute1, itemModel);
                    AddAttributeToModel(i.CatalogAttribute2, itemModel);
                    AddAttributeToModel(i.CatalogAttribute3, itemModel);


                    return itemModel;
                }).ToList()
            };
        }

        private void AddAttributeToModel(int? attributeId, BasketItemViewModel itemModel)
        {
            if (!attributeId.HasValue)
                return;
            var attr = _attributeRepository.GetById(attributeId.Value);
            itemModel.Attributes.Add(new BasketItemAttributeViewModel
            {
                Id = attr.Id,
                Name = attr.Name,
                Type = attr.Type
            });
        }

        private Basket ViewModelToBasket(BasketViewModel model)
        {
            var basket = new Basket
            {
                BuyerId = model.BuyerId
            };
            foreach (var item in model.Items)
            {

                basket.AddItem(item.ProductId, item.UnitPrice, item.Quantity, item.Attributes.Select(x => x.Id).ToList());
            }
            return basket;
        }

        private async Task<Basket> GetOrCreateBasketForUser(string userName)
        {
            var basketSpec = new BasketWithItemsSpecification(userName);
            var baskets = (await _repository.ListAsync(basketSpec));
            if (baskets == null || baskets.Count == 0)
            {
                return await CreateBasketForUser(userName);
            }

            //Delete previous Baskets
            if(baskets.Count > 1)
            {
                for (int i = 0; i < baskets.Count - 1; i++)
                {
                    await _repository.DeleteAsync(baskets[i]);
                }
            }

            return baskets.LastOrDefault();
        }

        private async Task<Basket> CreateBasketForUser(string userId)
        {
            var basket = new Basket() { BuyerId = userId };
            await _repository.AddAsync(basket);
            return basket;
        }
    }
}
