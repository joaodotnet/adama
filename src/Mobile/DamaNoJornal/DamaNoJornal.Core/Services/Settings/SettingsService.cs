using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DamaNoJornal.Core.Services.Settings
{
    public class SettingsService : ISettingsService
    {
        #region Setting Constants

        private const string AccessToken = "access_token";
        private const string IdToken = "id_token";
        private const string IdUseMocks = "use_mocks";
        private const string IdUrlBase = "url_base";
        private const string IdUseFakeLocation = "use_fake_location";
        private const string IdLatitude = "latitude";
        private const string IdLongitude = "longitude";
        private const string IdAllowGpsLocation = "allow_gps_location";
        private const string IdJueUserId = "jue_user_id";
        private const string IdSueUserId = "sue_user_id";
        private const string IdMotherUserId = "mother_user_id";
        private readonly string AccessTokenDefault = string.Empty;
        private readonly string IdTokenDefault = string.Empty;
        private readonly bool UseMocksDefault = true;
        private readonly bool UseFakeLocationDefault = false;
        private readonly bool AllowGpsLocationDefault = false;
        private readonly string JueUserIdDefault = string.Empty;
        private readonly string SueUserIdDefault = string.Empty;
        private readonly string MotherUserIdDefault = string.Empty;
        private readonly double FakeLatitudeDefault = 47.604610d;
        private readonly double FakeLongitudeDefault = -122.315752d;
        private readonly string UrlBaseDefault = GlobalSetting.Instance.BaseEndpoint;

        #endregion

        #region Settings Properties

        public string AuthAccessToken
        {
            get => GetValueOrDefault(AccessToken, AccessTokenDefault);
            set => AddOrUpdateValue(AccessToken, value);
        }

        public string AuthIdToken
        {
            get => GetValueOrDefault(IdToken, IdTokenDefault);
            set => AddOrUpdateValue(IdToken, value);
        }

        public bool UseMocks
        {
            get => GetValueOrDefault(IdUseMocks, UseMocksDefault);
            set => AddOrUpdateValue(IdUseMocks, value);
        }

        public string UrlBase
        {
            get => GetValueOrDefault(IdUrlBase, UrlBaseDefault);
            set => AddOrUpdateValue(IdUrlBase, value);
        }

        public bool UseFakeLocation
        {
            get => GetValueOrDefault(IdUseFakeLocation, UseFakeLocationDefault);
            set => AddOrUpdateValue(IdUseFakeLocation, value);
        }

        public string Latitude
        {
            get => GetValueOrDefault(IdLatitude, FakeLatitudeDefault.ToString());
            set => AddOrUpdateValue(IdLatitude, value);
        }

        public string Longitude
        {
            get => GetValueOrDefault(IdLongitude, FakeLongitudeDefault.ToString());
            set => AddOrUpdateValue(IdLongitude, value);
        }

        public bool AllowGpsLocation
        {
            get => GetValueOrDefault(IdAllowGpsLocation, AllowGpsLocationDefault);
            set => AddOrUpdateValue(IdAllowGpsLocation, value);
        }
        public string JueUserId
        {
            get => GetValueOrDefault(IdJueUserId, JueUserIdDefault);
            set => AddOrUpdateValue(IdJueUserId, value);
        }
        public string SueUserId
        {
            get => GetValueOrDefault(IdSueUserId, SueUserIdDefault);
            set => AddOrUpdateValue(IdSueUserId, value);
        }
        public string MotherUserId
        {
            get => GetValueOrDefault(IdMotherUserId, MotherUserIdDefault);
            set => AddOrUpdateValue(IdMotherUserId, value);
        }


        #endregion

        #region Public Methods

        public Task AddOrUpdateValue(string key, bool value) => AddOrUpdateValueInternal(key, value);
        public Task AddOrUpdateValue(string key, string value) => AddOrUpdateValueInternal(key, value);
        public bool GetValueOrDefault(string key, bool defaultValue) => GetValueOrDefaultInternal(key, defaultValue);
        public string GetValueOrDefault(string key, string defaultValue) => GetValueOrDefaultInternal(key, defaultValue);

        #endregion

        #region Internal Implementation

        async Task AddOrUpdateValueInternal<T>(string key, T value)
        {
            if (value == null)
            {
                await Remove(key);
            }

            Application.Current.Properties[key] = value;
            try
            {
                await Application.Current.SavePropertiesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to save: " + key, " Message: " + ex.Message);
            }
        }

        T GetValueOrDefaultInternal<T>(string key, T defaultValue = default(T))
        {
            object value = null;
            if (Application.Current.Properties.ContainsKey(key))
            {
                value = Application.Current.Properties[key];
            }
            return null != value ? (T)value : defaultValue;
        }

        async Task Remove(string key)
        {
            try
            {
                if (Application.Current.Properties[key] != null)
                {
                    Application.Current.Properties.Remove(key);
                    await Application.Current.SavePropertiesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to remove: " + key, " Message: " + ex.Message);
            }
        }

        #endregion
    }
}