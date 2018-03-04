using ApplicationCore.Interfaces;
using ApplicationCore.Entities.OrderAggregate;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using System.Collections.Generic;
using Ardalis.GuardClauses;

namespace ApplicationCore.Services
{
    public class OrderService : IOrderService
    {
        private readonly IAsyncRepository<Order> _orderRepository;
        private readonly IBasketRepository _basketRepository;
        private readonly IAsyncRepository<CatalogItem> _itemRepository;

        public OrderService(IBasketRepository basketRepository,
            IAsyncRepository<CatalogItem> itemRepository,
            IAsyncRepository<Order> orderRepository)
        {
            _orderRepository = orderRepository;
            _basketRepository = basketRepository;
            _itemRepository = itemRepository;
        }

        public async Task CreateOrderAsync(int basketId, Address shippingAddress, decimal shippingCost)
        {
            //TODO: check price
            var basket = await _basketRepository.GetByIdWithItemsAsync(basketId);
            Guard.Against.NullBasket(basketId, basket);
            var items = new List<OrderItem>();
            foreach (var item in basket.Items)
            {
                var catalogItem = await _itemRepository.GetByIdAsync(item.CatalogItemId);
                var itemOrdered = new CatalogItemOrdered(catalogItem.Id, catalogItem.Name, catalogItem.PictureUri);
                var orderItem = new OrderItem(itemOrdered, item.UnitPrice, item.Quantity);
                foreach (var attribute in item.Details)
                {
                    orderItem.Details.Add(new OrderItemDetail
                    {
                        AttributeType = attribute.CatalogAttribute.Type,
                        AttributeName = attribute.CatalogAttribute.Name
                    });
                }
                items.Add(orderItem);
            }
            var order = new Order(basket.BuyerId, shippingAddress, items, shippingCost);

            await _orderRepository.AddAsync(order);
        }
    }
}
