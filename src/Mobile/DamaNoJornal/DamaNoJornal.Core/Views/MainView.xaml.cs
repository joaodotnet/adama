using DamaNoJornal.Core.Models.Catalog;
using DamaNoJornal.Core.Models.Navigation;
using DamaNoJornal.Core.ViewModels;
using DamaNoJornal.Core.ViewModels.Base;
using Xamarin.Forms;

namespace DamaNoJornal.Core.Views
{
    public partial class MainView : TabbedPage
    {
        private object _parameter = null;
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
