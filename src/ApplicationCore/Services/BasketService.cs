using ApplicationCore.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using ApplicationCore.Specifications;
using ApplicationCore.Entities;
using System.Linq;
using Ardalis.GuardClauses;

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

            if(attrIds == null)
            {
                attrIds = new List<int>();
                var specification = new CatalogAttrFilterSpecification(catalogItemId);
                var item = _itemRepository.GetSingleBySpec(specification);
                var group = item.CatalogAttributes.GroupBy(x => x.Type);
                foreach (var attribute in group)
                {
                    attrIds.Add(attribute.First().Id);
                }
            }

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
    }
}
