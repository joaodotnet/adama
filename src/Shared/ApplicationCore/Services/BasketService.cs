using ApplicationCore.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using ApplicationCore.Specifications;
using ApplicationCore.Entities;
using System.Linq;
using Ardalis.GuardClauses;
using ApplicationCore.DTOs;
using ApplicationCore.Entities.BasketAggregate;
using System;

namespace ApplicationCore.Services
{
    public class BasketService : IBasketService
    {
        private readonly IAsyncRepository<Basket> _basketRepository;
        private readonly IAppLogger<BasketService> _logger;
        private readonly IAsyncRepository<CatalogItem> _itemRepository;        

        public BasketService(IAsyncRepository<Basket> basketRepository,
            IAsyncRepository<CatalogItem> itemRepository,
            IAppLogger<BasketService> logger)
        {
            _basketRepository = basketRepository;
            _logger = logger;
            _itemRepository = itemRepository;
        }

        public async Task AddItemToBasket(int basketId, int catalogItemId, decimal price, int quantity, int? option1 = null, int? option2 = null, int? option3 = null, string customizeName = null, string customizeSide = null, bool addToExistingItem = false)
        {
            var basket = await _basketRepository.GetByIdAsync(basketId);            

            basket.AddItem(catalogItemId, price, quantity, option1, option2, option3, customizeName, customizeSide, addToExistingItem);

            await _basketRepository.UpdateAsync(basket);
        }

        public async Task AddCustomizeItemToBasket(int basketId, int catalogTypeId, string description, string textOrName, string colors, int quantity)
        {
            var basket = await _basketRepository.GetByIdAsync(basketId);

            basket.AddCustomizeItem(catalogTypeId, description, textOrName, colors, quantity);

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
                    item.UpdatedDate = DateTime.Now;
                }
            }
            await _basketRepository.UpdateAsync(basket);
        }

        public async Task TransferBasketAsync(string anonymousId, string userName, bool isGuest)
        {
            Guard.Against.NullOrEmpty(anonymousId, nameof(anonymousId));
            Guard.Against.NullOrEmpty(userName, nameof(userName));            

            var basketSpec = new BasketWithItemsSpecification(anonymousId);
            var basket = (await _basketRepository.ListAsync(basketSpec)).LastOrDefault();
            if (basket == null) return;

            if (!isGuest && basket.Items?.Count > 0)
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
            basket.UpdatedDate = DateTime.Now;
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

            if (basket.Items.Any(x => x.CatalogTypeId.HasValue))
                return null;
            DeliveryTimeDTO deliveryTime = new DeliveryTimeDTO
            {
                Min = 2,
                Max = 3,
                Unit = DeliveryTimeUnitType.Days
            };
            foreach (var item in basket.Items)
            {
                var spec = new CatalogTypeFilterSpecification(item.CatalogItemId);
                var product = await _itemRepository.GetSingleBySpecAsync(spec);

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

        public async Task<(int?, int?, int?)> GetFirstOptionFromAttributeAsync(int catalogItemId)
        {
            var options = (default(int?), default(int?), default(int?));

            var spec = new CatalogTypeFilterSpecification(catalogItemId);
            var product = await _itemRepository.GetSingleBySpecAsync(spec);

            var group = product.CatalogAttributes.GroupBy(x => x.Type);
            foreach (var attribute in group)
            {
                if(!options.Item1.HasValue)
                    options.Item1 = attribute.First().Id;
                else if (!options.Item2.HasValue)
                    options.Item2 = attribute.First().Id;
                else if (!options.Item3.HasValue)
                    options.Item3 = attribute.First().Id;
            }

            return options;
        }

        public async Task AddObservationAsync(int basketId, string message)
        {
            var basket = await _basketRepository.GetByIdAsync(basketId);
            basket.Observations = message;
            await _basketRepository.UpdateAsync(basket);
        }

        public async Task RemoveObservationsAsync(int basketId)
        {
            var basket = await _basketRepository.GetByIdAsync(basketId);
            if (basket != null)
            {
                basket.Observations = null;
                await _basketRepository.UpdateAsync(basket);
            }
        }
    }
}
