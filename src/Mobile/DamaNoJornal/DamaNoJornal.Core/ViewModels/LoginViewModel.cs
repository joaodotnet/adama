using DamaNoJornal.Core.Models.Location;
using DamaNoJornal.Core.Models.User;
using DamaNoJornal.Core.Services.Identity;
using DamaNoJornal.Core.Services.OpenUrl;
using DamaNoJornal.Core.Services.Settings;
using DamaNoJornal.Core.Validations;
using DamaNoJornal.Core.ViewModels.Base;
using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace DamaNoJornal.Core.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private ValidatableObject<string> _userName;
        private ValidatableObject<string> _password;
        private bool _isMock;
        private bool _isValid;
        private bool _isLogin;
        private string _authUrl;

        private ISettingsService _settingsService;
        private IOpenUrlService _openUrlService;
        private IIdentityService _identityService;
        //private ObservableCollection<Place> _places;
        private Place _place;

        public LoginViewModel(
            ISettingsService settingsService,
            IOpenUrlService openUrlService,
            IIdentityService identityService)
        {
            _settingsService = settingsService;
            _openUrlService = openUrlService;
            _identityService = identityService;

            _userName = new ValidatableObject<string>();
            _password = new ValidatableObject<string>();

            InvalidateMock();
            AddValidations();
        }

        public ValidatableObject<string> UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
                RaisePropertyChanged(() => UserName);
            }
        }

        public ValidatableObject<string> Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                RaisePropertyChanged(() => Password);
            }
        }

        public bool IsMock
        {
            get
            {
                return _isMock;
            }
            set
            {
                _isMock = value;
                RaisePropertyChanged(() => IsMock);
            }
        }

        public bool IsValid
        {
            get
            {
                return _isValid;
            }
            set
            {
                _isValid = value;
                RaisePropertyChanged(() => IsValid);
            }
        }

        public bool IsLogin
        {
            get
            {
                return _isLogin;
            }
            set
            {
                _isLogin = value;
                RaisePropertyChanged(() => IsLogin);
            }
        }

        public string LoginUrl
        {
            get
            {
                return _authUrl;
            }
            set
            {
                _authUrl = value;
                RaisePropertyChanged(() => LoginUrl);
            }
        }

        //public ObservableCollection<Place> Places
        //{
        //    get { return _places; }
        //    set
        //    {
        //        _places = value;
        //        RaisePropertyChanged(() => Places);
        //    }
        //}

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

        //public Place PlaceSelected => place1;        



        public ICommand MockSignInCommand => new Command<string>(async (user) => await MockSignInAsync(user));

        public ICommand SignInCommand => new Command(async () => await SignInAsync());

        public ICommand RegisterCommand => new Command(Register);

        public ICommand NavigateCommand => new Command<string>(async (url) => await NavigateAsync(url));

        public ICommand SettingsCommand => new Command(async () => await SettingsAsync());

        public ICommand ValidateUserNameCommand => new Command(() => ValidateUserName());

        public ICommand ValidatePasswordCommand => new Command(() => ValidatePassword());

        public override Task InitializeAsync(object navigationData)
        {
            if (navigationData is LogoutParameter)
            {
                var logoutParameter = (LogoutParameter)navigationData;

                if (logoutParameter.Logout)
                {
                    Logout();
                }
            }
            
            return base.InitializeAsync(navigationData);
        }

        private async Task MockSignInAsync(string user)
        {
            IsBusy = true;
            IsValid = true;
            //bool isValid = Validate();
            bool isAuthenticated = true;

            //if (isValid)
            //{
            //    try
            //    {
            //        await Task.Delay(10);

            //        isAuthenticated = true;
            //    }
            //    catch (Exception ex)
            //    {
            //        Debug.WriteLine($"[SignIn] Error signing in: {ex}");
            //    }
            //}
            //else
            //{
            //    IsValid = false;
            //}

            if (isAuthenticated)
            {
                var staffList = await _identityService.GetStaffUsersAsync();
                switch (user)
                {
                    case "jue":
                        _settingsService.AuthAccessToken = GlobalSetting.Instance.AuthToken = staffList.SingleOrDefault(x => x.Email == "jue@damanojornal.com").UserId;
                        break;
                    case "sue":
                        _settingsService.AuthAccessToken = GlobalSetting.Instance.AuthToken = staffList.SingleOrDefault(x => x.Email == "sue@damanojornal.com").UserId;
                        break;
                    case "sonia":
                        _settingsService.AuthAccessToken = GlobalSetting.Instance.AuthToken = staffList.SingleOrDefault(x => x.Email == "sonia@damanojornal.com").UserId;
                        break;
                    default:                        
                        break;
                }
                _settingsService.PlaceId = PlaceSelected.Id.ToString();
                await NavigationService.NavigateToAsync<MainViewModel>();
                await NavigationService.RemoveLastFromBackStackAsync();
            }

            IsBusy = false;
        }

        private async Task SignInAsync()
        {
            IsBusy = true;

            await Task.Delay(10);

            LoginUrl = _identityService.CreateAuthorizationRequest();

            IsValid = true;
            IsLogin = true;
            IsBusy = false;
        }

        private void Register()
        {
            _openUrlService.OpenUrl(GlobalSetting.Instance.RegisterWebsite);
        }

        private void Logout()
        {
            var authIdToken = _settingsService.AuthIdToken;
            var logoutRequest = _identityService.CreateLogoutRequest(authIdToken);

            if (!string.IsNullOrEmpty(logoutRequest))
            {
                // Logout
                LoginUrl = logoutRequest;
            }

            //Simulate logout
            //if (_settingsService.UseMocks)
            //{
            _settingsService.AuthAccessToken = string.Empty;
            _settingsService.AuthIdToken = string.Empty;
            _settingsService.PlaceId = string.Empty;
            //}

            _settingsService.UseFakeLocation = false;
        }

        private async Task NavigateAsync(string url)
        {
            var unescapedUrl = System.Net.WebUtility.UrlDecode(url);

            if (unescapedUrl.Equals(GlobalSetting.Instance.LogoutCallback))
            {
                _settingsService.AuthAccessToken = string.Empty;
                _settingsService.AuthIdToken = string.Empty;
                IsLogin = false;
                LoginUrl = _identityService.CreateAuthorizationRequest();
            }
            else if (unescapedUrl.Contains(GlobalSetting.Instance.IdentityCallback))
            {
                var authResponse = new AuthorizeResponse(url);
                if (!string.IsNullOrWhiteSpace(authResponse.Code))
                {
                    var userToken = await _identityService.GetTokenAsync(authResponse.Code);
                    string accessToken = userToken.AccessToken;

                    if (!string.IsNullOrWhiteSpace(accessToken))
                    {
                        _settingsService.AuthAccessToken = accessToken;
                        _settingsService.AuthIdToken = authResponse.IdentityToken;
                        await NavigationService.NavigateToAsync<MainViewModel>();
                        await NavigationService.RemoveLastFromBackStackAsync();
                    }
                }
            }
        }

        private async Task SettingsAsync()
        {
            await NavigationService.NavigateToAsync<SettingsViewModel>();
        }

        private bool Validate()
        {
            bool isValidUser = ValidateUserName();
            bool isValidPassword = ValidatePassword();

            return isValidUser && isValidPassword;
        }

        private bool ValidateUserName()
        {
            return _userName.Validate();
        }

        private bool ValidatePassword()
        {
            return _password.Validate();
        }

        private void AddValidations()
        {
            _userName.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "A username is required." });
            _password.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "A password is required." });
        }

        public void InvalidateMock()
        {
            //JG Don't use identity
            IsMock = true; // _settingsService.UseMocks;
        }
    }
}