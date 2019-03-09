using ApplicationCore.Interfaces;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using System.Linq;
using System.Collections.Generic;
using ApplicationCore.Specifications;
using DamaWeb.Interfaces;
using DamaWeb.ViewModels;
using DamaWeb.Extensions;
using ApplicationCore.Entities.BasketAggregate;
using System;

namespace DamaWeb.Services
{
    public class BasketViewModelService : IBasketViewModelService
    {
        private readonly IAsyncRepository<Basket> _basketRepository;
        private readonly IUriComposer _uriComposer;
        private readonly IRepository<CatalogItem> _itemRepository;
        private readonly IAsyncRepository<CatalogType> _typeRepository;

        public BasketViewModelService(IAsyncRepository<Basket> basketRepository,
            IRepository<CatalogItem> itemRepository,
            IAsyncRepository<CatalogType> typeRepository,
            IUriComposer uriComposer)
        {
            _basketRepository = basketRepository;
            _uriComposer = uriComposer;
            _itemRepository = itemRepository;
            _typeRepository = typeRepository;
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
            var result = await GetBasketItemsAndShipping(basket.Items);
            viewModel.Items = result.Items; 
            viewModel.DefaultShippingCost = result.ShippingCost;

            return viewModel;
        }

        private async Task<(List<BasketItemViewModel> Items, decimal ShippingCost)> GetBasketItemsAndShipping(IReadOnlyCollection<BasketItem> basketItems)
        {
            var items = new List<BasketItemViewModel>();
            decimal shippingCost = 0;
            foreach (var item in basketItems)
            {
                var itemModel = new BasketItemViewModel()
                {
                    Id = item.Id,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    CatalogItemId = item.CatalogItemId,
                    CustomizeName = item.CustomizeName
                };

                if (item.CatalogItemId != 0)
                {
                    var spec = new CatalogTypeFilterSpecification(item.CatalogItemId);
                    var catalogItem = _itemRepository.GetSingleBySpec(spec);
                    if (catalogItem != null)
                    {
                        itemModel.PictureUrl = _uriComposer.ComposePicUri(catalogItem.PictureUri);
                        itemModel.ProductName = catalogItem.Name;
                        itemModel.Sku = catalogItem.Sku;
                        itemModel.Slug = catalogItem.Slug;

                        foreach (var attr in catalogItem.CatalogAttributes)
                        {
                            if ((item.CatalogAttribute1.HasValue && item.CatalogAttribute1 == attr.Id) ||
                                (item.CatalogAttribute2.HasValue && item.CatalogAttribute2 == attr.Id) ||
                                (item.CatalogAttribute3.HasValue && item.CatalogAttribute3 == attr.Id))
                                itemModel.Attributes.Add(new AttributeViewModel
                                {
                                    Name = attr.Name,
                                    Label = EnumHelper<AttributeType>.GetDisplayValue(attr.Type)
                                });
                        }

                        if (catalogItem != null && catalogItem.CatalogType.ShippingCost > shippingCost)
                            shippingCost = catalogItem.CatalogType.ShippingCost;
                    }
                }
                else if (item.CatalogTypeId.HasValue)
                {
                    itemModel.IsFromCustomize = true;
                    var typeEntity = await _typeRepository.GetByIdAsync(item.CatalogTypeId.Value);
                    if (typeEntity != null)
                    {
                        itemModel.PictureUrl = typeEntity.PictureUri;
                        itemModel.ProductName = $"Personalização {typeEntity.Description}";
                    }
                }

                items.Add(itemModel);
            }
            return (items,shippingCost);
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
