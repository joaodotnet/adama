using DamaNoJornal.Core.Helpers;
using DamaNoJornal.Core.Models.Location;
using DamaNoJornal.Core.Models.Marketing;
using DamaNoJornal.Core.Models.User;
using DamaNoJornal.Core.Services.Marketing;
using DamaNoJornal.Core.Services.Settings;
using DamaNoJornal.Core.Services.User;
using DamaNoJornal.Core.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace DamaNoJornal.Core.ViewModels
{
    public class EventViewModel : ViewModelBase
    {
        private readonly ISettingsService _settingsService;
        private readonly IUserService _userService;
        private Place _place;

        public EventViewModel(ISettingsService settingsService, IUserService userService)
        {
            _settingsService = settingsService;
            _userService = userService;
        }

        public Place PlaceSelected
        {
            get => _place;
            set
            {
                _place = value;
                RaisePropertyChanged(() => PlaceSelected);
            }
        }

        public List<Place> Places => GlobalSetting.Places;

        public ICommand EventDetailCommand => new Command(async() => await EventDetailAsync());

        public override async Task InitializeAsync(object navigationData)
        {
            IsBusy = true;
            PlaceSelected = GlobalSetting.Place2;
            // Get user
            //var user = await _userService.GetUserInfoAsync(_settingsService.AuthAccessToken);
            //PictureUri = Utils.GetLoginPicturiSource(user.Email);
            //Name = $"Olá {user.Name} {user.LastName}";
            IsBusy = false;            
        }
        private async Task EventDetailAsync()
        {
            await NavigationService.NavigateToAsync<EventDetailViewModel>(PlaceSelected);
        }
    }
}