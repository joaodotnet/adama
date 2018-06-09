﻿using DamaNoJornal.Core.Models.Basket;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DamaNoJornal.Core.Services.Order
{
    public interface IOrderService
    {
        Task CreateOrderAsync(Core.Models.Orders.Order newOrder, string token);
        Task<ObservableCollection<Core.Models.Orders.Order>> GetOrdersAsync(string buyerId, string token);
        Task<Core.Models.Orders.Order> GetOrderAsync(int orderId, string token);
        Task<bool> CancelOrderAsync(int orderId, string token);
        BasketCheckout MapOrderToBasket(Core.Models.Orders.Order order);
    }
}