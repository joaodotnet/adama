﻿using DamaNoJornal.Core.Models.Location;
using DamaNoJornal.Core.Services.Dependency;
using DamaNoJornal.Core.Services.Location;
using DamaNoJornal.Core.Services.Settings;
using DamaNoJornal.Core.ViewModels.Base;
using DamaNoJornal.Services;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace DamaNoJornal
{
    public partial class App : Application
    {
        ISettingsService _settingsService;

        public App()
        {
            InitializeComponent();

            InitApp();
            if (Device.RuntimePlatform == Device.UWP)
            {
                InitNavigation();
            }
        }

        private void InitApp()
        {
            _settingsService = ViewModelLocator.Resolve<ISettingsService>();
            _settingsService.UseMocks = false;
            if(IsLoginExpired())
                _settingsService.AuthAccessToken = string.Empty;
            if (!_settingsService.UseMocks)
                ViewModelLocator.UpdateDependencies(_settingsService.UseMocks);
        }

        private bool IsLoginExpired()
        {
            if(!string.IsNullOrEmpty(_settingsService.LoginSince))
            {
                var loginDate = Convert.ToDateTime(_settingsService.LoginSince);
                if ((DateTime.Now - loginDate) > new TimeSpan(12, 0, 0))
                    return true;
                return false;
            }
            return true;
        }

        private Task InitNavigation()
        {
            var navigationService = ViewModelLocator.Resolve<INavigationService>();
            return navigationService.InitializeAsync();
        }

        protected override async void OnStart()
        {
            base.OnStart();

            if (Device.RuntimePlatform != Device.UWP)
            {
                await InitNavigation();
            }
            //if (_settingsService.AllowGpsLocation && !_settingsService.UseFakeLocation)
            //{
            //    await GetGpsLocation();
            //}
            //if (!_settingsService.UseMocks && !string.IsNullOrEmpty(_settingsService.AuthAccessToken))
            //{
            //    await SendCurrentLocation();
            //}

            base.OnResume();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        private async Task GetGpsLocation()
        {
            var dependencyService = ViewModelLocator.Resolve<IDependencyService>();
            var locator = dependencyService.Get<ILocationServiceImplementation>();

            if (locator.IsGeolocationEnabled && locator.IsGeolocationAvailable)
            {
                locator.DesiredAccuracy = 50;

                try
                {
                    var position = await locator.GetPositionAsync();
                    _settingsService.Latitude = position.Latitude.ToString();
                    _settingsService.Longitude = position.Longitude.ToString();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
            else
            {
                _settingsService.AllowGpsLocation = false;
            }
        }

        private async Task SendCurrentLocation()
        {
            var location = new Location
            {
                Latitude = double.Parse(_settingsService.Latitude, CultureInfo.InvariantCulture),
                Longitude = double.Parse(_settingsService.Longitude, CultureInfo.InvariantCulture)
            };

            var locationService = ViewModelLocator.Resolve<ILocationService>();
            await locationService.UpdateUserLocation(location, _settingsService.AuthAccessToken);
        }
    }
}