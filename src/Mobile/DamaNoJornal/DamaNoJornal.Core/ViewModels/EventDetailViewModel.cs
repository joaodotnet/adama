using DamaNoJornal.Core.Models.Location;
using DamaNoJornal.Core.Models.Orders;
using DamaNoJornal.Core.Services.Order;
using DamaNoJornal.Core.Services.Settings;
using DamaNoJornal.Core.ViewModels.Base;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace DamaNoJornal.Core.ViewModels
{
    public class EventDetailViewModel : ViewModelBase
    {
        private readonly ISettingsService _settingsService;
        private readonly IOrderService _ordersService;

        private Place _place;
        private string _title;
        private string _subTitle;

        public EventDetailViewModel(ISettingsService settingsService, IOrderService ordersService)
        {
            _settingsService = settingsService;
            _ordersService = ordersService;
        }

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                RaisePropertyChanged(() => Title);
            }
        }

        public string SubTitle
        {
            get => _subTitle;
            set
            {
                _subTitle = value;
                RaisePropertyChanged(() => SubTitle);
            }
        }

        //public Order Order
        //{
        //    get => _order;
        //    set
        //    {
        //        _order = value;
        //        RaisePropertyChanged(() => Order);
        //    }
        //}

        public override async Task InitializeAsync(object navigationData)
        {
            if (navigationData is Place)
            {
                IsBusy = true;

                var place = navigationData as Place;
                Title = place.Name;

                // Get order detail info
                var authToken = _settingsService.AuthAccessToken;

                List<Order> orders = await _ordersService.GetOrderByPlaceAsync(place.Id, authToken);
                //Total 
                var total = orders
                    .SelectMany(x => x.OrderItems)
                    .Sum(i => i.UnitPrice * i.Quantity);
                SubTitle = $"Total de Vendas: {total}€";
                IsBusy = false;
            }
        }
    }
}