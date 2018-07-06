using ApplicationCore.Interfaces;
using ApplicationCore.Entities.OrderAggregate;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using System.Collections.Generic;
using Ardalis.GuardClauses;
using ApplicationCore.Specifications;
using System.Linq;

namespace ApplicationCore.Services
{
    public class OrderService : IOrderService
    {
        private readonly IAsyncRepository<Order> _orderRepository;
        private readonly IAsyncRepository<CustomizeOrder> _customizeOrderRepository;
        private readonly IBasketRepository _basketRepository;
        private readonly IAsyncRepository<CatalogItem> _itemRepository;

        public OrderService(IBasketRepository basketRepository,
            IAsyncRepository<CatalogItem> itemRepository,
            IAsyncRepository<Order> orderRepository,
            IAsyncRepository<CustomizeOrder> customizeOrderRepository)
        {
            _orderRepository = orderRepository;
            _basketRepository = basketRepository;
            _itemRepository = itemRepository;
            _customizeOrderRepository = customizeOrderRepository;
        }

        public async Task<Order> CreateOrderAsync(int basketId, string phoneNumber, int? taxNumber, Address shippingAddress, Address billingAddress, bool useBillingSameAsShipping, decimal shippingCost)
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
                //foreach (var attribute in item.Details)
                //{
                //    orderItem.Details.Add(new OrderItemDetail
                //    {
                //        AttributeType = attribute.CatalogAttribute.Type,
                //        AttributeName = attribute.CatalogAttribute.Name
                //    });
                //}
                items.Add(orderItem);
            }
            var order = new Order(basket.BuyerId, phoneNumber, taxNumber, shippingAddress, billingAddress, useBillingSameAsShipping, items, shippingCost);

            return await _orderRepository.AddAsync(order);
            
        }       

        public async Task UpdateOrderState(int id, OrderStateType orderState, bool isCustomizeOrder = false)
        {
            if (!isCustomizeOrder)
            {
                var order = await _orderRepository.GetByIdAsync(id);
                if (order != null)
                {
                    order.OrderState = orderState;
                    await _orderRepository.UpdateAsync(order);
                }
            }
            else
            {
                var order = await _customizeOrderRepository.GetByIdAsync(id);
                if (order != null)
                {
                    order.OrderState = orderState;
                    await _customizeOrderRepository.UpdateAsync(order);
                }
            }
        }
        public async Task<Order> GetOrderAsync(int id)
        {
            var spec = new OrdersSpecification(id);
            return (await _orderRepository.ListAsync(spec)).FirstOrDefault();
        }

        public async Task<List<Order>> GetOrdersAsync(string buyerId)
        {
            var spec = new OrdersSpecification(buyerId);
            return await _orderRepository.ListAsync(spec);
        }
    }
}
