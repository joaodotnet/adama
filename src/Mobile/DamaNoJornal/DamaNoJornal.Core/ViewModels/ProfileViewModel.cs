using DamaNoJornal.Core.Helpers;
using DamaNoJornal.Core.Models.Marketing;
using DamaNoJornal.Core.Models.User;
using DamaNoJornal.Core.Services.Marketing;
using DamaNoJornal.Core.Services.Settings;
using DamaNoJornal.Core.Services.User;
using DamaNoJornal.Core.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace DamaNoJornal.Core.ViewModels
{
    public class ProfileViewModel : ViewModelBase
    {
        private readonly ISettingsService _settingsService;
        private readonly IUserService _userService;
        private string _pictureUri;
        private string _name;

        public ProfileViewModel(ISettingsService settingsService, IUserService userService)
        {
            _settingsService = settingsService;
            _userService = userService;
        }

        public ICommand LogoutCommand => new Command(async () => await LogoutAsync());

        public string PictureUri
        {
            get => _pictureUri;
            set
            {
                _pictureUri = value;
                RaisePropertyChanged(() => PictureUri);
            }
        }
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        public override async Task InitializeAsync(object navigationData)
        {
            IsBusy = true;
            // Get user
            var user = await _userService.GetUserInfoAsync(_settingsService.AuthAccessToken);
            PictureUri = Utils.GetLoginPicturiSource(user.Email);
            Name = $"Olá {user.Name} {user.LastName}";
            IsBusy = false;            
        }

        private async Task LogoutAsync()
        {
            IsBusy = true;

            // Logout
            await NavigationService.NavigateToAsync<LoginViewModel>(new LogoutParameter { Logout = true });
            await NavigationService.RemoveBackStackAsync();

            IsBusy = false;
        }
    }
}