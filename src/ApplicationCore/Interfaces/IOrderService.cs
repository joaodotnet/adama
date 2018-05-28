using ApplicationCore.Entities.OrderAggregate;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(int basketId, int? taxNumber, Address shippingAddress, Address billingAddress, bool useBillingSameAsShipping, decimal shippingCost);
        Task UpdateOrderState(int id, OrderStateType orderState, bool isCustomizeOrder = false);
        Task<Order> GetOrderAsync(int id);
        Task<List<Order>> GetOrdersAsync(string buyerId);
    }
}
