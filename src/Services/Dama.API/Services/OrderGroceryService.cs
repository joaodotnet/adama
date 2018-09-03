using ApplicationCore.Interfaces;
using ApplicationCore.Entities.OrderAggregate;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using System.Collections.Generic;
using Ardalis.GuardClauses;
using ApplicationCore.Specifications;
using System.Linq;
using Dama.API.Interfaces;

namespace Dama.API.Services
{
    public class OrderGroceryService : IOrderGroceryService
    {
        private readonly IGroceryAsyncRepository<Order> _orderRepository;
        private readonly IGroceryAsyncRepository<CustomizeOrder> _customizeOrderRepository;
        private readonly IGroceryRepository _groceryRepository;
        private readonly IGroceryAsyncRepository<CatalogItem> _itemRepository;
        //private readonly IGroceryAsyncRepository<CatalogItem> _itemSyncRepository;

        public OrderGroceryService(IGroceryRepository basketRepository,
            IGroceryAsyncRepository<CatalogItem> itemRepository,
            IGroceryAsyncRepository<Order> orderRepository,
            IGroceryAsyncRepository<CustomizeOrder> customizeOrderRepository)
            /*IRepository<CatalogItem> itemSyncRepository*/
        {
            _orderRepository = orderRepository;
            _groceryRepository = basketRepository;
            _itemRepository = itemRepository;
            _customizeOrderRepository = customizeOrderRepository;
            //_itemSyncRepository = itemSyncRepository;
        }

        public async Task<Order> CreateOrderAsync(int basketId, string phoneNumber, int? taxNumber, Address shippingAddress, Address billingAddress, bool useBillingSameAsShipping, decimal shippingCost, string customerEmail)
        {
            //TODO: check price
            var basket = await _groceryRepository.GetByIdWithItemsAsync(basketId);
            Guard.Against.NullBasket(basketId, basket);
            var items = new List<OrderItem>();
            foreach (var item in basket.Items)
            {
                var catalogItem = await _itemRepository.GetByIdAsync(item.CatalogItemId);
                var itemOrdered = new CatalogItemOrdered(catalogItem.Id, catalogItem.Name, catalogItem.PictureUri);
                var orderItem = new OrderItem(itemOrdered, item.UnitPrice, item.Quantity, item.CatalogAttribute1, item.CatalogAttribute2, item.CatalogAttribute3, item.CustomizeName, item.CustomizeSide);
                items.Add(orderItem);
            }
            var order = new Order(basket.BuyerId, phoneNumber, taxNumber, shippingAddress, billingAddress, useBillingSameAsShipping, items, shippingCost, customerEmail);

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

        public async Task<List<CatalogAttribute>> GetOrderAttributesAsync(int orderId, int orderItemId)
        {
            var specOrders = new OrdersSpecification(orderId);
            var order = (await _orderRepository.ListAsync(specOrders)).FirstOrDefault();
            var orderItem = order.OrderItems.SingleOrDefault(x => x.Id == orderItemId);

            var spec = new CatalogAttrFilterSpecification(orderItem.ItemOrdered.CatalogItemId);
            var product = (await _itemRepository.ListAsync(spec)).FirstOrDefault();

            var list = new List<CatalogAttribute>();
            foreach (var item in product.CatalogAttributes)
            {
                if ((orderItem.CatalogAttribute1.HasValue && orderItem.CatalogAttribute1 == item.Id) ||
                       (orderItem.CatalogAttribute2.HasValue && orderItem.CatalogAttribute2 == item.Id) ||
                       (orderItem.CatalogAttribute3.HasValue && orderItem.CatalogAttribute3 == item.Id))
                    list.Add(item);
            }
            return list;
        }

        public List<CatalogAttribute> GetOrderAttributes(int catalogItemId, int? catalogAttribute1, int? catalogAttribute2, int? catalogAttribute3)
        {
            throw new System.NotImplementedException();
        }
    }
}
