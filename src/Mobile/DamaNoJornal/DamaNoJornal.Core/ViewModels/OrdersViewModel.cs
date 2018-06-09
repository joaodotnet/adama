using DamaNoJornal.Core.Extensions;
using DamaNoJornal.Core.Models.Orders;
using DamaNoJornal.Core.Models.User;
using DamaNoJornal.Core.Services.Order;
using DamaNoJornal.Core.Services.Settings;
using DamaNoJornal.Core.Services.User;
using DamaNoJornal.Core.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace DamaNoJornal.Core.ViewModels
{
    public class OrdersViewModel : ViewModelBase
    {
        private readonly ISettingsService _settingsService;
        private readonly IOrderService _orderService;
        private ObservableCollection<Order> _orders;
        private IUserService _userService;

        public OrdersViewModel(ISettingsService settingsService, IOrderService orderService, IUserService userService)
        {
            _settingsService = settingsService;
            _orderService = orderService;
            _userService = userService;
        }

        public ObservableCollection<Order> Orders
        {
            get { return _orders; }
            set
            {
                _orders = value;
                RaisePropertyChanged(() => Orders);
            }
        }        

        //public ICommand LogoutCommand => new Command(async () => await LogoutAsync());

        public ICommand OrderDetailCommand => new Command<Order>(async (order) => await OrderDetailAsync(order));

        public override async Task InitializeAsync(object navigationData)
        {
            IsBusy = true;

            // Get orders
            var authToken = _settingsService.AuthAccessToken;
            var userInfo = await _userService.GetUserInfoAsync(authToken);
            var orders = await _orderService.GetOrdersAsync(userInfo.UserId, authToken);
            Orders = orders.ToObservableCollection();

            IsBusy = false;
        }

        //private async Task LogoutAsync()
        //{
        //    IsBusy = true;

        //    // Logout
        //    await NavigationService.NavigateToAsync<LoginViewModel>(new LogoutParameter { Logout = true });
        //    await NavigationService.RemoveBackStackAsync();

        //    IsBusy = false;
        //}

        private async Task OrderDetailAsync(Order order)
        {
            await NavigationService.NavigateToAsync<OrderDetailViewModel>(order);
        }
    }
}