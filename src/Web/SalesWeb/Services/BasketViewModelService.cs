using ApplicationCore.Interfaces;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using System.Linq;
using System.Collections.Generic;
using ApplicationCore.Specifications;
using SalesWeb.Interfaces;
using SalesWeb.ViewModels;
using SalesWeb.Extensions;
using ApplicationCore.Entities.BasketAggregate;
using System;

namespace SalesWeb.Services
{
    public class BasketViewModelService : IBasketViewModelService
    {
        private readonly IAsyncRepository<Basket> _basketRepository;
        private readonly IUriComposer _uriComposer;
        private readonly IAsyncRepository<CatalogItem> _itemRepository;

        public BasketViewModelService(IAsyncRepository<Basket> basketRepository,
            IAsyncRepository<CatalogItem> itemRepository,
            IUriComposer uriComposer)
        {
            _basketRepository = basketRepository;
            _uriComposer = uriComposer;
            _itemRepository = itemRepository;
        }

        public async Task<BasketViewModel> GetOrCreateBasketForUser(string userName)
        {
            var basketSpec = new BasketWithItemsSpecification(userName);
            var basket = (await _basketRepository.ListAsync(basketSpec)).LastOrDefault();

            if (basket == null)
            {
                return await CreateBasketForUser(userName);
            }
            return await CreateViewModelFromBasketAsync(basket);
        }

        private async Task<BasketViewModel> CreateViewModelFromBasketAsync(Basket basket)
        {
            var viewModel = new BasketViewModel();
            viewModel.Id = basket.Id;
            viewModel.BuyerId = basket.BuyerId;
            viewModel.Items = await GetBasketItemsAsync(basket.Items);
            return viewModel;
        }

        private async Task<List<BasketItemViewModel>> GetBasketItemsAsync(IReadOnlyCollection<BasketItem> basketItems)
        {
            var items = new List<BasketItemViewModel>();
            foreach (var i in basketItems)
            {

                var itemModel = new BasketItemViewModel()
                {
                    Id = i.Id,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    CatalogItemId = i.CatalogItemId,
                    CustomizeName = i.CustomizeName
                };
                var spec = new CatalogAttrFilterSpecification(i.CatalogItemId);
                var item = await _itemRepository.GetSingleBySpecAsync(spec);
                if (item != null)
                {
                    itemModel.PictureUrl = _uriComposer.ComposePicUri(item.PictureUri);
                    itemModel.ProductName = item.Name;
                }

                foreach (var attr in item.CatalogAttributes)
                {
                    if ((i.CatalogAttribute1.HasValue && i.CatalogAttribute1 == attr.Id) ||
                        (i.CatalogAttribute2.HasValue && i.CatalogAttribute2 == attr.Id) ||
                        (i.CatalogAttribute3.HasValue && i.CatalogAttribute3 == attr.Id))
                        itemModel.Attributes.Add(new AttributeViewModel
                        {
                            Name = attr.Name,
                            Label = EnumHelper<AttributeType>.GetDisplayValue(attr.Type)
                        });
                }
                items.Add(itemModel);
            }
            return items;
        }
        private async Task<BasketViewModel> CreateBasketForUser(string userId)
        {
            var basket = new Basket() { BuyerId = userId, CreatedDate = DateTime.Now };
            await _basketRepository.AddAsync(basket);

            return new BasketViewModel()
            {
                BuyerId = basket.BuyerId,
                Id = basket.Id,
                Items = new List<BasketItemViewModel>()
            };
        }
    }
}
