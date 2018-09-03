using ApplicationCore.Entities;
using ApplicationCore.Entities.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(int basketId, string phoneNumber, int? taxNumber, Address shippingAddress, Address billingAddress, bool useBillingSameAsShipping, decimal shippingCost, string customerEmail = null);
        Task UpdateOrderState(int id, OrderStateType orderState, bool isCustomizeOrder = false);
        Task<Order> GetOrderAsync(int id);
        Task<List<Order>> GetOrdersAsync(string buyerId);
        Task<List<CatalogAttribute>> GetOrderAttributesAsync(int orderId, int orderItemId);
        List<CatalogAttribute> GetOrderAttributes(int catalogItemId, int? catalogAttribute1, int? catalogAttribute2, int? catalogAttribute3);
    }
}
