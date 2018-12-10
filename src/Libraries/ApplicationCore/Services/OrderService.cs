using ApplicationCore.Interfaces;
using ApplicationCore.Entities.OrderAggregate;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using System.Collections.Generic;
using Ardalis.GuardClauses;
using ApplicationCore.Specifications;
using System.Linq;
using ApplicationCore.DTOs;
using System;
using ApplicationCore.Exceptions;

namespace ApplicationCore.Services
{
    public class OrderService : IOrderService
    {
        private readonly IAsyncRepository<Order> _orderRepository;
        private readonly IAsyncRepository<CustomizeOrder> _customizeOrderRepository;
        private readonly IBasketRepository _basketRepository;
        private readonly IAsyncRepository<CatalogItem> _itemRepository;
        private readonly IRepository<CatalogItem> _itemSyncRepository;
        private readonly IInvoiceService _invoiceService;

        public OrderService(IBasketRepository basketRepository,
            IAsyncRepository<CatalogItem> itemRepository,
            IAsyncRepository<Order> orderRepository,
            IAsyncRepository<CustomizeOrder> customizeOrderRepository,
            IRepository<CatalogItem> itemSyncRepository,
            IInvoiceService invoiceService)
        {
            _orderRepository = orderRepository;
            _basketRepository = basketRepository;
            _itemRepository = itemRepository;
            _customizeOrderRepository = customizeOrderRepository;
            _itemSyncRepository = itemSyncRepository;
            _invoiceService = invoiceService;
        }

        public async Task<Order> CreateOrderAsync(int basketId, string phoneNumber, int? taxNumber, Address shippingAddress, Address billingAddress, bool useBillingSameAsShipping, decimal shippingCost, string customerEmail = null, bool registerInvoice = false, PaymentType paymentType = PaymentType.CASH)
        {
            //TODO: check price
            var basket = await _basketRepository.GetByIdWithItemsAsync(basketId);
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
            var savedOrder = await _orderRepository.AddAsync(order);

            if (registerInvoice)
            {
                savedOrder.OrderState = OrderStateType.SUBMITTED;
                SageResponseDTO response;

                try
                {
                    response = await _invoiceService.RegisterInvoiceAsync(savedOrder);                   
                }
                catch (Exception ex)
                {
                    throw new RegisterInvoiceException(ex.ToString());
                }

                if (response.InvoiceId.HasValue)
                {
                    savedOrder.SalesInvoiceId = response.InvoiceId.Value;
                    savedOrder.SalesInvoiceNumber = response.InvoiceNumber;

                    await _orderRepository.UpdateAsync(savedOrder);

                    //Generate Payment
                    try
                    {
                        var responsePayment = await _invoiceService.RegisterPaymentAsync(savedOrder.SalesInvoiceId.Value, savedOrder.Total(), paymentType);
                        if (responsePayment.PaymentId.HasValue)
                        {
                            savedOrder.SalesPaymentId = responsePayment.PaymentId.Value;
                            await _orderRepository.UpdateAsync(savedOrder);
                        }
                        else
                            throw new RegisterInvoiceException($"Fatura gerada com sucesso mas com erro de pagamento: {responsePayment?.Message}");
                    }
                    catch (Exception ex)
                    {
                        throw new RegisterInvoiceException($"Fatura gerada com sucesso mas com erro de pagamento: {ex}");
                    }
                }
                else //Something wrong
                {
                    throw new RegisterInvoiceException(response.Message);
                }
            }

            return savedOrder;
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
            var spec = new CatalogAttrFilterSpecification(catalogItemId);
            var product = _itemSyncRepository.GetSingleBySpec(spec);

            var list = new List<CatalogAttribute>();
            foreach (var item in product.CatalogAttributes)
            {
                if ((catalogAttribute1.HasValue && catalogAttribute1 == item.Id) ||
                       (catalogAttribute2.HasValue && catalogAttribute2 == item.Id) ||
                       (catalogAttribute3.HasValue && catalogAttribute3 == item.Id))
                    list.Add(item);
            }
            return list;
        }

        public async Task UpdateOrderInvoiceAsync(int id, long? invoiceId, string invoiceNumber)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order != null)
            {
                order.SalesInvoiceId = invoiceId;
                order.SalesInvoiceNumber = invoiceNumber;
                await _orderRepository.UpdateAsync(order);
            }
        }
        public async Task UpdateOrderBillingAsync(int id, int? taxNumber, string customerEmail, Address billingAddress)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order != null)
            {
                order.UpdateBillingInfo(taxNumber, customerEmail, billingAddress);

                await _orderRepository.UpdateAsync(order);
            }
        }

    }
}
