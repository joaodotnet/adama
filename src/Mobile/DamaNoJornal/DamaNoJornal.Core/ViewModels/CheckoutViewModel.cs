using DamaNoJornal.Core.Models.Basket;
using DamaNoJornal.Core.Models.Catalog;
using DamaNoJornal.Core.Models.Navigation;
using DamaNoJornal.Core.Models.Orders;
using DamaNoJornal.Core.Models.User;
using DamaNoJornal.Core.Services.Basket;
using DamaNoJornal.Core.Services.Order;
using DamaNoJornal.Core.Services.Settings;
using DamaNoJornal.Core.Services.User;
using DamaNoJornal.Core.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace DamaNoJornal.Core.ViewModels
{
    public class CheckoutViewModel : ViewModelBase
    {
        private ObservableCollection<BasketItem> _orderItems;
        private Order _order;

        private ISettingsService _settingsService;
        private IBasketService _basketService;
        private IOrderService _orderService;
        private IUserService _userService;
        private Address _shippingAddress;
        private bool _createInvoice;
        private string _taxNumber;

        public CheckoutViewModel(
            ISettingsService settingsService,
            IBasketService basketService,
            IOrderService orderService,
            IUserService userService)
        {
            _settingsService = settingsService;
            _basketService = basketService;
            _orderService = orderService;
            _userService = userService;
        }

        public ObservableCollection<BasketItem> OrderItems
        {
            get { return _orderItems; }
            set
            {
                _orderItems = value;
                RaisePropertyChanged(() => OrderItems);
            }
        }

        public Order Order
        {
            get { return _order; }
            set
            {
                _order = value;
                RaisePropertyChanged(() => Order);
            }
        }

        public Address BillingAddress
        {
            get { return _shippingAddress; }
            set
            {
                _shippingAddress = value;
                RaisePropertyChanged(() => BillingAddress);
            }
        }

        public string TaxNumber
        {
            get { return _taxNumber; }
            set
            {
                _taxNumber = value;
                RaisePropertyChanged(() => TaxNumber);
            }
        }

        public bool CreateInvoice
        {
            get => _createInvoice;
            set
            {
                _createInvoice = value;

                RaisePropertyChanged(() => CreateInvoice);
            }
        }

        public ICommand CheckoutCommand => new Command(async () => await CheckoutAsync());

        public override async Task InitializeAsync(object navigationData)
        {
            if (navigationData is ObservableCollection<BasketItem>)
            {
                IsBusy = true;

                // Get navigation data
                var orderItems = ((ObservableCollection<BasketItem>)navigationData);

                OrderItems = orderItems;

                var authToken = _settingsService.AuthAccessToken;
                var userInfo = await _userService.GetUserInfoAsync(authToken);

                // Create Payment Info
                var paymentInfo = new PaymentInfo
                {
                    CardNumber = userInfo?.CardNumber,
                    CardHolderName = userInfo?.CardHolder,
                    CardType = new CardType { Id = 3, Name = "MasterCard" },
                    SecurityNumber = userInfo?.CardSecurityNumber
                };

                BillingAddress = new Address();

                // Create new Order
                var place = GlobalSetting.Places.SingleOrDefault(x => x.Id.ToString() == _settingsService.PlaceId);
                Order = new Order
                {
                    BuyerId = userInfo.Email,
                    OrderItems = CreateOrderItems(orderItems),
                    OrderStatus = OrderStatus.SUBMITTED,
                    OrderDate = DateTime.Now,
                    CardHolderName = paymentInfo.CardHolderName,
                    CardNumber = paymentInfo.CardNumber,
                    CardSecurityNumber = paymentInfo.SecurityNumber,
                    CardExpiration = DateTime.Now.AddYears(5),
                    CardTypeId = paymentInfo.CardType.Id,
                    ShippingCountry = place.Country,
                    ShippingStreet = place.Name,
                    ShippingCity = place.City,
                    ShippingZipCode = place.PostalCode,                    
                };

                IsBusy = false;
            }
        }

        private async Task CheckoutAsync()
        {
            try
            {
                var authToken = _settingsService.AuthAccessToken;
                var userInfo = await _userService.GetUserInfoAsync(authToken);

                //var basket = _orderService.MapOrderToBasket(Order);
                //basket.RequestId = Guid.NewGuid();

                if(CreateInvoice)
                {
                    Order.CreateInvoice = true;
                    Order.BillingName = BillingAddress.Name;
                    Order.BillingStreet = BillingAddress.Street;
                    Order.BillingPostalCode = BillingAddress.PostalCode;
                    Order.BillingCity = BillingAddress.City;
                    Order.BillingCountry = "Portugal";
                    Order.TaxNumber = !string.IsNullOrEmpty(TaxNumber) && Int32.TryParse(TaxNumber, out int taxNumber) ? taxNumber : default(int?);
                }

                var result = await _orderService.CreateOrderAsync(Order, authToken);
               
                // Clean Basket
                await _basketService.ClearBasketAsync(userInfo.UserId, authToken);

                // Reset Basket badge
                //var basketViewModel = ViewModelLocator.Resolve<BasketViewModel>();
                //basketViewModel.BadgeCount = 0;

                // Navigate to Orders
                await NavigationService.NavigateToAsync<MainViewModel>(new TabParameter { TabIndex = 1 });
                await NavigationService.RemoveLastFromBackStackAsync();

                // Show Dialog
                await DialogService.ShowAlertAsync($"Encomenda efectuado com sucesso! {result.ResultMessage}", "Checkout", "Ok");
                await NavigationService.RemoveLastFromBackStackAsync();
            }
            catch(Exception ex)
            {
                await DialogService.ShowAlertAsync($"Ocorreu um erro: {ex.Message}", "Oops!", "Ok");
            }
        }

        private List<OrderItem> CreateOrderItems(ObservableCollection<BasketItem> basketItems)
        {
            var orderItems = new List<OrderItem>();

            foreach (var basketItem in basketItems)
            {
                if (!string.IsNullOrEmpty(basketItem.ProductName))
                {
                    orderItems.Add(new OrderItem
                    {
                        OrderId = null,
                        ProductId = basketItem.ProductId,
                        ProductName = basketItem.ProductName,
                        PictureUrl = basketItem.PictureUrl,
                        Quantity = basketItem.Quantity,
                        UnitPrice = basketItem.UnitPrice,
                        Details = basketItem.Attributes.Select(x => new OrderItemDetail
                        {
                            Id = x.Id,
                            AttributeType = x.Type,
                            AttributeName = $"{AttributeTypeHelper.GetTypeDescription(x.Type)} {x.Name}"
                        }).ToList()
                    });
                }
            }

            return orderItems;
        }

        private decimal CalculateTotal(List<OrderItem> orderItems)
        {
            decimal total = 0;

            foreach (var orderItem in orderItems)
            {
                total += (orderItem.Quantity * orderItem.UnitPrice);
            }

            return total;
        }
    }
}