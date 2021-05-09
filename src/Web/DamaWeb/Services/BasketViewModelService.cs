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
using Microsoft.Extensions.Options;
using ApplicationCore;

namespace DamaWeb.Services
{
    public class BasketViewModelService : IBasketViewModelService
    {
        private readonly IAsyncRepository<Basket> _basketRepository;
        private readonly IUriComposer _uriComposer;
        private readonly CatalogSettings _settings;
        private readonly IAsyncRepository<CatalogItem> _itemRepository;
        private readonly IAsyncRepository<CatalogType> _typeRepository;
        private readonly IAsyncRepository<ShippingPriceWeight> _priceWeightRepository;

        public BasketViewModelService(IAsyncRepository<Basket> basketRepository,
            IAsyncRepository<CatalogItem> itemRepository,
            IAsyncRepository<CatalogType> typeRepository,
            IAsyncRepository<ShippingPriceWeight> priceWeightRepository,
            IUriComposer uriComposer,
            IOptions<CatalogSettings> settings)
        {
            _basketRepository = basketRepository;
            _uriComposer = uriComposer;
            _settings = settings.Value;
            _itemRepository = itemRepository;
            _typeRepository = typeRepository;
            _priceWeightRepository = priceWeightRepository;
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
            viewModel.ShowShippingCost = false;           
            viewModel.Observations = basket.Observations;

            return viewModel;
        }

        private async Task<(List<BasketItemViewModel> Items, decimal ShippingCost)> GetBasketItemsAndShipping(IReadOnlyCollection<BasketItem> basketItems)
        {
            var items = new List<BasketItemViewModel>();
            decimal shippingCost = _settings.DefaultShippingCost;
            int totalWeight = 0;
            bool allItemsWithWeight = true;
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
                    var catalogItem = await _itemRepository.GetSingleBySpecAsync(spec);
                    if (catalogItem != null)
                    {
                        itemModel.PictureUrl = _uriComposer.ComposePicUri(catalogItem.PictureUri);
                        itemModel.ProductName = GetProductPrincipalName(catalogItem.Name);
                        itemModel.ProductName2 = GetProductSecondaryName(catalogItem.Name);
                        itemModel.Sku = catalogItem.Sku;
                        itemModel.Slug = catalogItem.Slug;

                        foreach (var attr in catalogItem.Attributes)
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
                        if (!catalogItem.CatalogType.Weight.HasValue || catalogItem.CatalogType.Weight == 0)
                            allItemsWithWeight = false;
                        else
                            totalWeight += catalogItem.CatalogType.Weight.Value * item.Quantity;
                        //if (catalogItem != null && catalogItem.CatalogType.ShippingCost > shippingCost)
                        //    shippingCost = catalogItem.CatalogType.ShippingCost;
                    }
                }
                else if (item.CatalogTypeId.HasValue)
                {
                    itemModel.IsFromCustomize = true;
                    var typeEntity = await _typeRepository.GetByIdAsync(item.CatalogTypeId.Value);
                    if (typeEntity != null)
                    {
                        itemModel.PictureUrl = typeEntity.PictureUri;
                        itemModel.ProductName = $"Personalização {typeEntity.Name}";
                    }
                }

                items.Add(itemModel);
            }
            if (allItemsWithWeight)
                shippingCost = await CalculateShippingCostAsync(totalWeight);
            return (items, shippingCost);
        }
        private string GetProductPrincipalName(string name)
        {
            if (name.IndexOf("-") > 0)
                return name.Substring(0, name.LastIndexOf("-"));
            return name;
        }

        private string GetProductSecondaryName(string name)
        {
            if (name.IndexOf("-") > 0)
                return name.Substring(name.LastIndexOf("-") + 1);
            return "";
        }


        private async Task<decimal> CalculateShippingCostAsync(int totalWeight)
        {
            var table = await _priceWeightRepository.ListAllAsync();
            if(table.Count > 0)
            {
                foreach (var item in table)
                {
                    if (item.MaxWeight.HasValue)
                    {
                        if (totalWeight > item.MinWeight && totalWeight <= item.MaxWeight.Value)
                            return item.Price;
                    }
                    else if (totalWeight > item.MinWeight)
                        return item.Price;
                    
                }
            }
            return _settings.DefaultShippingCost;
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
