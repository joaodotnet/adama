using DamaNoJornal.Core.Models.Navigation;
using DamaNoJornal.Core.ViewModels.Base;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace DamaNoJornal.Core.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public ICommand SettingsCommand => new Command(async () => await SettingsAsync());

        public override Task InitializeAsync(object navigationData)
        {
            IsBusy = true;

            if (navigationData is TabParameter)
            {
                // Change selected application tab
                //var tabIndex = ((TabParameter)navigationData).TabIndex;
                var tabParameter = ((TabParameter)navigationData);
                MessagingCenter.Send(this, MessageKeys.ChangeTab, tabParameter);
            }

            return base.InitializeAsync(navigationData);
        }

        private async Task SettingsAsync()
        {
            await NavigationService.NavigateToAsync<SettingsViewModel>();
        }
    }
}