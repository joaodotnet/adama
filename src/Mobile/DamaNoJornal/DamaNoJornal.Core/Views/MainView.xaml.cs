using System;
using DamaNoJornal.Core.Models.Catalog;
using DamaNoJornal.Core.Models.Navigation;
using DamaNoJornal.Core.Services.Settings;
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
                        CurrentPage = ProfileView;
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
			await ((ProfileViewModel)ProfileView.BindingContext).InitializeAsync(null);

            var info = GetProfileInfo();
            this.Children.Add(new Page());
            this.Children[3].Title = info.Item1;
            this.Children[3].Icon = info.Item2;            
        }

        private (string,string) GetProfileInfo()
        {
            _settingsService = ViewModelLocator.Resolve<ISettingsService>();
            string title = "Olá ";
            string iconPath = "";
            switch (_settingsService.AuthAccessToken)
            {
                case GlobalSetting.JueAuthToken:
                    title += "João";
                    iconPath = "Assets\\img-joao.png";
                    break;
                case GlobalSetting.SueAuthToken:
                    title += "Susana";
                    iconPath = "Assets\\img-sue.png";
                    break;
                case GlobalSetting.SoniaAuthToken:
                    title += "Sónia";
                    iconPath = "Assets\\img-sonia.png";
                    break;
                default:
                    break;
            }
            return (title, iconPath);
        }

        protected override async void OnCurrentPageChanged()
        {
            base.OnCurrentPageChanged();

            if (CurrentPage is BasketView)
            {
                // Force basket view refresh every time we access it
                await (BasketView.BindingContext as ViewModelBase).InitializeAsync(null);
            }
            else if (CurrentPage is ProfileView)
            {
                // Force profile view refresh every time we access it
                await (ProfileView.BindingContext as ViewModelBase).InitializeAsync(null);
            }
        }        
    }
}
