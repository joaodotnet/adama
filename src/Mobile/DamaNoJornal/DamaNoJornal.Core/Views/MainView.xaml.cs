using System;
using DamaNoJornal.Core.Helpers;
using DamaNoJornal.Core.Models.Catalog;
using DamaNoJornal.Core.Models.Navigation;
using DamaNoJornal.Core.Services.Settings;
using DamaNoJornal.Core.Services.User;
using DamaNoJornal.Core.ViewModels;
using DamaNoJornal.Core.ViewModels.Base;
using Xamarin.Forms;

namespace DamaNoJornal.Core.Views
{
    public partial class MainView : TabbedPage
    {
        private object _parameter = null;
        private ISettingsService _settingsService;
        public MainView()
        {
            InitializeComponent();

            MessagingCenter.Unsubscribe<MainViewModel, int>(this, MessageKeys.ChangeTab);
            MessagingCenter.Subscribe<MainViewModel, TabParameter>(this, MessageKeys.ChangeTab, (sender, arg) =>
            {
                switch (arg.TabIndex)
                {
                    case 0:
                        CurrentPage = HomeView;
                        _parameter = arg.ParameterObj;
                        break;
                    case 1:
                        CurrentPage = OrdersView;
                        break;
                    case 2:
                        CurrentPage = BasketView;
                        break;
                }
            });
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

			await ((CatalogViewModel)HomeView.BindingContext).InitializeAsync(_parameter);
			await ((BasketViewModel)BasketView.BindingContext).InitializeAsync(null);
			await ((OrdersViewModel)OrdersView.BindingContext).InitializeAsync(null);
            await ((ProfileViewModel)ProfileView.BindingContext).InitializeAsync(null);
            await ((EventViewModel)EventView.BindingContext).InitializeAsync(null);

            _settingsService = ViewModelLocator.Resolve<ISettingsService>();
            var userService = ViewModelLocator.Resolve<IUserService>();
            var user = await userService.GetUserInfoAsync(_settingsService.AuthAccessToken);
            //this.Children.Add(new Page());
            this.Children[3].Title = $"Olá {user.Name}";
            this.Children[3].Icon = Utils.GetLoginPicturiSource(user.Email);
        }

        protected override async void OnCurrentPageChanged()
        {
            base.OnCurrentPageChanged();

            if (CurrentPage is BasketView)
            {
                // Force basket view refresh every time we access it
                await (BasketView.BindingContext as ViewModelBase).InitializeAsync(null);
            }
            else if (CurrentPage is OrdersView)
            {
                // Force profile view refresh every time we access it
                await (OrdersView.BindingContext as ViewModelBase).InitializeAsync(null);
            }
        }        
    }
}
