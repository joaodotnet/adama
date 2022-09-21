﻿using ApplicationCore.Entities;
using ApplicationCore.Entities.OrderAggregate;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Ardalis.GuardClauses;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.DTOs;
using System;
using ApplicationCore.Exceptions;

namespace ApplicationCore.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<CustomizeOrder> _customizeOrderRepository;
        private readonly IBasketRepository _basketRepository;
        private readonly IRepository<CatalogItem> _itemRepository;
        private readonly IRepository<CatalogType> _catalogRepository;
        private readonly IInvoiceService _invoiceService;

        public OrderService(IBasketRepository basketRepository,
            IRepository<CatalogItem> itemRepository,
            IRepository<Order> orderRepository,
            IRepository<CustomizeOrder> customizeOrderRepository,
            IRepository<CatalogType> catalogRepository,
            IInvoiceService invoiceService)
        {
            _orderRepository = orderRepository;
            _basketRepository = basketRepository;
            _itemRepository = itemRepository;
            _customizeOrderRepository = customizeOrderRepository;
            _catalogRepository = catalogRepository;
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
                if (!item.CatalogTypeId.HasValue)
                {
                    var catalogItem = await _itemRepository.GetByIdAsync(item.CatalogItemId);
                    var itemOrdered = new CatalogItemOrdered(catalogItem.Id, catalogItem.Name, catalogItem.PictureUri);
                    var orderItem = new OrderItem(itemOrdered, item.UnitPrice, item.Quantity, item.CatalogAttribute1, item.CatalogAttribute2, item.CatalogAttribute3, item.CustomizeName, item.CustomizeSide, new CustomizeItemOrdered());
                    items.Add(orderItem);
                }
                else
                {
                    var catalogType = await _catalogRepository.GetByIdAsync(item.CatalogTypeId.Value);
                    var itemOrdered = new CatalogItemOrdered(0, null, null);
                    var customizeItem = new CustomizeItemOrdered(
                        item.CatalogTypeId.Value,
                        item.CustomizeDescription,
                        item.CustomizeName,
                        item.CustomizeColors,
                        catalogType.Name,
                        catalogType.PictureUri);

                    var orderItem = new OrderItem(itemOrdered, 0, item.Quantity, null, null, null, null, null, customizeItem);

                    items.Add(orderItem);
                }
            }
            var order = new Order(basket.BuyerId, phoneNumber, taxNumber, shippingAddress, billingAddress, useBillingSameAsShipping, items, shippingCost, basket.Observations, basket.Discount, customerEmail);
            var savedOrder = await _orderRepository.AddAsync(order);

            if (registerInvoice)
            {
                savedOrder.OrderState = OrderStateType.SUBMITTED;
                SageResponseDTO response;
                //From sales rename product name

                foreach (var item in savedOrder.OrderItems.Where(x => x.ItemOrdered.CatalogItemId == -1).ToList())
                {
                    item.ItemOrdered.Rename(item.CustomizeName);
                }

                try
                {
                    response = await _invoiceService.RegisterInvoiceAsync(SageApplicationType.SALESWEB, savedOrder);
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
                        var responsePayment = await _invoiceService.RegisterPaymentAsync(SageApplicationType.SALESWEB, savedOrder.SalesInvoiceId.Value, savedOrder.Total(), paymentType);
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
                    order.UpdateOrderState(orderState);
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

        public async Task<List<(int OrderItemId, List<CatalogAttribute> Attributes)>> GetOrderAttributesAsync(List<OrderItem> orderItems)
        {
            //var specOrders = new OrdersSpecification(orderId);
            //var order = (await _orderRepository.ListAsync(specOrders)).FirstOrDefault();
            //var orderItem = order.OrderItems.SingleOrDefault(x => x.Id == orderItemId);

            if (orderItems.Any(x => x.CustomizeItem != null))
                return null;

            var list = new List<(int OrderItemId, List<CatalogAttribute> Attributes)>();
            foreach (var orderItem in orderItems)
            {
                var spec = new CatalogAttrFilterSpecification(orderItem.ItemOrdered.CatalogItemId);
                var product = await _itemRepository.FirstOrDefaultAsync(spec);
                var listItem = (orderItem.Id, new List<CatalogAttribute>());
                foreach (var item in product.Attributes)
                {
                    if ((orderItem.CatalogAttribute1.HasValue && orderItem.CatalogAttribute1 == item.Id) ||
                            (orderItem.CatalogAttribute2.HasValue && orderItem.CatalogAttribute2 == item.Id) ||
                            (orderItem.CatalogAttribute3.HasValue && orderItem.CatalogAttribute3 == item.Id))
                        listItem.Item2.Add(item);
                }
                list.Add(listItem);
            }
            return list;
        }

        public async Task<List<CatalogAttribute>> GetOrderAttributesAsync(int catalogItemId, int? catalogAttribute1, int? catalogAttribute2, int? catalogAttribute3)
        {
            var spec = new CatalogAttrFilterSpecification(catalogItemId);
            var product = await _itemRepository.FirstOrDefaultAsync(spec);
            var list = new List<CatalogAttribute>();
            if (product != null)
            {
                foreach (var item in product.Attributes)
                {
                    if ((catalogAttribute1.HasValue && catalogAttribute1 == item.Id) ||
                           (catalogAttribute2.HasValue && catalogAttribute2 == item.Id) ||
                           (catalogAttribute3.HasValue && catalogAttribute3 == item.Id))
                        list.Add(item);
                }
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

        public async Task UpdateOrderItemsPrice(int orderId, List<Tuple<int, decimal>> items)
        {
            var order = await GetOrderAsync(orderId);
            if(order != null && items?.Count > 0)
            {
                foreach (var item in items)
                {
                    var orderItem = order.OrderItems.SingleOrDefault(x => x.Id == item.Item1);
                    if (orderItem != null)
                        orderItem.UpdateItemPrice(item.Item2);
                }
                await _orderRepository.UpdateAsync(order);
            }
        }

    }
}
