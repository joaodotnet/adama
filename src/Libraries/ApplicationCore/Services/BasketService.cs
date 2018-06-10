﻿using ApplicationCore.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using ApplicationCore.Specifications;
using ApplicationCore.Entities;
using System.Linq;
using Ardalis.GuardClauses;
using ApplicationCore.DTOs;

namespace ApplicationCore.Services
{
    public class BasketService : IBasketService
    {
        private readonly IAsyncRepository<Basket> _basketRepository;
        private readonly IUriComposer _uriComposer;
        private readonly IAppLogger<BasketService> _logger;
        private readonly IRepository<CatalogItem> _itemRepository;        

        public BasketService(IAsyncRepository<Basket> basketRepository,
            IRepository<CatalogItem> itemRepository,
            IUriComposer uriComposer,
            IAppLogger<BasketService> logger)
        {
            _basketRepository = basketRepository;
            _uriComposer = uriComposer;
            this._logger = logger;
            _itemRepository = itemRepository;
        }

        public async Task AddItemToBasket(int basketId, int catalogItemId, decimal price, int quantity, List<int> attrIds = null)
        {
            var basket = await _basketRepository.GetByIdAsync(basketId);

            var specification = new CatalogAttrFilterSpecification(catalogItemId);
            var catalogItem = _itemRepository.GetSingleBySpec(specification);

            decimal attrsPrice = 0;
            if(attrIds == null)
            {
                attrIds = new List<int>();
                
                var group = catalogItem.CatalogAttributes.GroupBy(x => x.Type);
                foreach (var attribute in group)
                {
                    attrIds.Add(attribute.First().Id);
                    attrsPrice += attribute.First().Price ?? 0;
                }
            }
            else
            {
                foreach (var item in attrIds)
                {
                    var attrItem = catalogItem.CatalogAttributes.SingleOrDefault(x => x.Id == item);
                    if (attrItem != null)
                        attrsPrice += attrItem.Price ?? 0;
                }
            }

            //update price 
            price += attrsPrice;

            basket.AddItem(catalogItemId, price, quantity, attrIds);

            await _basketRepository.UpdateAsync(basket);
        }

        public async Task DeleteBasketAsync(int basketId)
        {
            var basket = await _basketRepository.GetByIdAsync(basketId);

            await _basketRepository.DeleteAsync(basket);
        }       

        public async Task<int> GetBasketItemCountAsync(string userName)
        {
            if(!string.IsNullOrEmpty(userName))
            {
                Guard.Against.NullOrEmpty(userName, nameof(userName));
                var basketSpec = new BasketWithItemsSpecification(userName);
                var basket = (await _basketRepository.ListAsync(basketSpec)).LastOrDefault();
                if (basket == null)
                {
                    _logger.LogInformation($"No basket found for {userName}");
                    return 0;
                }
                int count = basket.Items.Sum(i => i.Quantity);
                _logger.LogInformation($"Basket for {userName} has {count} items.");
                return count;
            }
            return 0;
        }

        public async Task SetQuantities(int basketId, Dictionary<string, int> quantities)
        {
            Guard.Against.Null(quantities, nameof(quantities));
            var basket = await _basketRepository.GetByIdAsync(basketId);
            Guard.Against.NullBasket(basketId, basket);
            foreach (var item in basket.Items)
            {
                if (quantities.TryGetValue(item.Id.ToString(), out var quantity))
                {
                    _logger.LogInformation($"Updating quantity of item ID:{item.Id} to {quantity}.");
                    item.Quantity = quantity;
                }
            }
            await _basketRepository.UpdateAsync(basket);
        }

        public async Task TransferBasketAsync(string anonymousId, string userName)
        {
            Guard.Against.NullOrEmpty(anonymousId, nameof(anonymousId));
            Guard.Against.NullOrEmpty(userName, nameof(userName));            

            var basketSpec = new BasketWithItemsSpecification(anonymousId);
            var basket = (await _basketRepository.ListAsync(basketSpec)).LastOrDefault();
            if (basket == null) return;

            if (basket.Items?.Count > 0)
            {
                //Delete previous baskets
                var basketSpecUser = new BasketWithItemsSpecification(userName);
                var listBasket = await _basketRepository.ListAsync(basketSpecUser);
                foreach (var item in listBasket)
                {
                    await _basketRepository.DeleteAsync(item);
                }
            }
            basket.BuyerId = userName;
            await _basketRepository.UpdateAsync(basket);            
        }
        public async Task DeleteItem(int basketId, int itemIndex)
        {
            var basket = await _basketRepository.GetByIdAsync(basketId);
            if (basket != null)
                basket.RemoveItem(itemIndex);
            await _basketRepository.UpdateAsync(basket);
        }

        public async Task<DeliveryTimeDTO> CalculateDeliveryTime(int basketId) //TODO: Create unit test
        {
            var basketSpec = new BasketWithItemsSpecification(basketId);
            var basket = (await _basketRepository.ListAsync(basketSpec)).SingleOrDefault();

            DeliveryTimeDTO deliveryTime = new DeliveryTimeDTO
            {
                Min = 2,
                Max = 3,
                Unit = DeliveryTimeUnitType.Days
            };
            foreach (var item in basket.Items)
            {
                var spec = new CatalogTypeFilterSpecification(item.CatalogItemId);
                var product = _itemRepository.GetSingleBySpec(spec);

                if(product.CatalogType.DeliveryTimeUnit > deliveryTime.Unit)
                {
                    deliveryTime.Min = product.CatalogType.DeliveryTimeMin;
                    deliveryTime.Max = product.CatalogType.DeliveryTimeMax;
                    deliveryTime.Unit = product.CatalogType.DeliveryTimeUnit;
                }
                else if(product.CatalogType.DeliveryTimeUnit == deliveryTime.Unit)
                {
                    deliveryTime.Min = product.CatalogType.DeliveryTimeMin > deliveryTime.Min ? product.CatalogType.DeliveryTimeMin : deliveryTime.Min;
                    deliveryTime.Max = product.CatalogType.DeliveryTimeMax > deliveryTime.Max ? product.CatalogType.DeliveryTimeMax : deliveryTime.Max;                    
                }                
            }
            return deliveryTime;
        }
    }
}