using DamaNoJornal.Core.Extensions;
using DamaNoJornal.Core.Models.Location;
using DamaNoJornal.Core.Models.Orders;
using DamaNoJornal.Core.Services.Order;
using DamaNoJornal.Core.Services.Settings;
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
    public class EventDetailViewModel : ViewModelBase
    {
        private readonly ISettingsService _settingsService;
        private readonly IOrderService _ordersService;

        private string _title;
        private string _subTitle;
        private ObservableCollection<OrderDay> _orders;

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

        public ObservableCollection<OrderDay> OrdersByDay
        {
            get => _orders;
            set
            {
                _orders = value;
                RaisePropertyChanged(() => OrdersByDay);
            }
        }

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

                var group = orders.OrderBy(x => x.OrderDate).GroupBy(x => x.OrderDate.Date);
                OrdersByDay = new ObservableCollection<OrderDay>();
                foreach (var ordersByDay in group)
                {
                    OrderDay orderDay = new OrderDay
                    {
                        Date = ordersByDay.Key,
                        Items = new ObservableCollection<OrderItem>()
                    };
                    int num = 0;
                    foreach (var order in ordersByDay)
                    {
                        foreach (var item in order.OrderItems)
                        {
                            item.Num = ++num;
                            orderDay.Items.Add(item);
                        }
                    }
                    OrdersByDay.Add(orderDay);
                }

                IsBusy = false;
            }
        }
    }

    public class OrderDay
    {
        public DateTime Date { get; set; }
        public ObservableCollection<OrderItem> Items { get; set; }

        public decimal DateTotal { get
            {
                return Items?.Sum(x => x.Total) ?? 0;
            }
        }
    }
}