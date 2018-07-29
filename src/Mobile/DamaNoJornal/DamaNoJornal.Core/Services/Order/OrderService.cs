using DamaNoJornal.Core.Extensions;
using DamaNoJornal.Core.Models.Basket;
using DamaNoJornal.Core.Models.Catalog;
using DamaNoJornal.Core.Models.Orders;
using DamaNoJornal.Core.Services.Order.Models;
using DamaNoJornal.Core.Services.RequestProvider;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace DamaNoJornal.Core.Services.Order
{
    public class OrderService : IOrderService
    {
        private readonly IRequestProvider _requestProvider;

        private const string ApiUrlBase = "api/v1/orders";

        public OrderService(IRequestProvider requestProvider)
        {
            _requestProvider = requestProvider;
        }

        public async Task CreateOrderAsync(Core.Models.Orders.Order newOrder, string token)
        {
            var builder = new UriBuilder(GlobalSetting.Instance.BaseEndpoint)
            {
                Path = ApiUrlBase
            };

            var uri = builder.ToString();
            var result = await _requestProvider.PostAsync(uri, newOrder, token);            
        }

        public async Task<ObservableCollection<Core.Models.Orders.Order>> GetOrdersAsync(string id, string token)
        {
            UriBuilder builder = new UriBuilder(GlobalSetting.Instance.BaseEndpoint);

            builder.Path = $"{ApiUrlBase}/all/{id}";

            string uri = builder.ToString();

            IEnumerable<Models.OrderEntity> orders =
                await _requestProvider.GetAsync<IEnumerable<Models.OrderEntity>>(uri, token);

            return MapToModelOrders(orders.OrderByDescending(x => x.OrderDate).ToList());

        }        

        public async Task<Core.Models.Orders.Order> GetOrderAsync(int orderId, string token)
        {
            try
            {
                UriBuilder builder = new UriBuilder(GlobalSetting.Instance.BaseEndpoint);

                builder.Path = $"{ApiUrlBase}/{orderId}";

                string uri = builder.ToString();

                OrderEntity order =
                    await _requestProvider.GetAsync<OrderEntity>(uri, token);

                return MapToOrder(order);
            }
            catch
            {
                return new Core.Models.Orders.Order();
            }
        }

        public BasketCheckout MapOrderToBasket(Core.Models.Orders.Order order)
        {
            return new BasketCheckout()
            {
                CardExpiration = order.CardExpiration,
                CardHolderName = order.CardHolderName,
                CardNumber = order.CardNumber,
                CardSecurityNumber = order.CardSecurityNumber,
                CardTypeId = order.CardTypeId,
                City = order.ShippingCity,
                State = order.ShippingState,
                Country = order.ShippingCountry,
                ZipCode = order.ShippingZipCode,
                Street = order.ShippingStreet
            };
        }

        public async Task<bool> CancelOrderAsync(int orderId, string token)
        {
            UriBuilder builder = new UriBuilder(GlobalSetting.Instance.BaseEndpoint);

            builder.Path = $"{ApiUrlBase}/cancel";

            var cancelOrderCommand = new CancelOrderCommand(orderId);

            string uri = builder.ToString();
            var header = "x-requestid";

            try
            {
                await _requestProvider.PutAsync(uri, cancelOrderCommand, token, header);
            }
            //If the status of the order has changed before to click cancel button, we will get
            //a BadRequest HttpStatus
            catch (HttpRequestExceptionEx ex) when (ex.HttpCode == System.Net.HttpStatusCode.BadRequest)
            {
                return false;
            }

            return true;
        }

        public async Task<List<Core.Models.Orders.Order>> GetOrderByPlaceAsync(int placeId, string authToken)
        {
            try
            {
                UriBuilder builder = new UriBuilder(GlobalSetting.Instance.BaseEndpoint);

                builder.Path = $"{ApiUrlBase}/place/{placeId}";

                string uri = builder.ToString();

                List<OrderEntity> orders =
                    await _requestProvider.GetAsync<List<OrderEntity>>(uri, authToken);

                return MapToOrder(orders);
            }
            catch
            {
                return new List<Core.Models.Orders.Order>();
            }
        }


        private ObservableCollection<Core.Models.Orders.Order> MapToModelOrders(IEnumerable<OrderEntity> orders)
        {
            ObservableCollection<Core.Models.Orders.Order> returnOrders = new ObservableCollection<Core.Models.Orders.Order>();
            foreach (var order in orders)
            {
                returnOrders.Add(MapToOrder(order));
            }
            return returnOrders;
        }

        private List<Core.Models.Orders.Order> MapToOrder(List<OrderEntity> orders)
        {
            var ordersViewModel = new List<Core.Models.Orders.Order>();
            foreach (var item in orders)
            {
                ordersViewModel.Add(MapToOrder(item));
            }
            return ordersViewModel;
        }

        private Core.Models.Orders.Order MapToOrder(OrderEntity order)
        {
            return new Core.Models.Orders.Order
            {
                OrderNumber = order.Id,
                OrderDate = order.OrderDate.DateTime,
                BuyerId = order.BuyerId,
                OrderStatus = order.OrderState,
                ShippingCity = order.ShipToAddress.City,
                ShippingCost = order.ShippingCost,
                ShippingCountry = order.ShipToAddress.Country,
                ShippingStreet = order.ShipToAddress.Street,
                ShippingZipCode = order.ShipToAddress.PostalCode,
                OrderItems = order.OrderItems.Select(i => new OrderItem
                {
                    ProductId = i.ItemOrdered.CatalogItemId,
                    PictureUrl = i.ItemOrdered.PictureUri,
                    ProductName = i.ItemOrdered.ProductName,
                    Quantity = i.Units,
                    UnitPrice = i.UnitPrice,
                    Details = i.Details.Select(d => new OrderItemDetail
                    {
                        Id = d.Id,
                        AttributeType = d.AttributeType,
                        AttributeName = $"{AttributeTypeHelper.GetTypeDescription(d.AttributeType)} {d.AttributeName}"
                    }).ToList()
                }).ToList()
            };
        }
    }
}