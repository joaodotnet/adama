namespace DamaApp.Core
{
    public class GlobalSetting
    {
        public const string AzureTag = "Azure";
        public const string MockTag = "Mock";
        public const string DefaultEndpoint = "http://localhost:50930"; // i.e.: "http://YOUR_IP" or "http://YOUR_DNS_NAME"
        public const string JGToken = "b77380e4-2f31-45f5-831c-8e5116d0475f";
        public const string SusanaToken = "b8d5d98b-3c61-4f21-89e0-e6bb2c22a634";
        public const string SoniaToken = "7989634d-86d6-4001-812c-b85ad9339e68";

        private string _baseEndpoint;
        private static readonly GlobalSetting _instance = new GlobalSetting();

        public GlobalSetting()
        {
            //AuthToken = "INSERT AUTHENTICATION TOKEN";
            BaseEndpoint = DefaultEndpoint;
        }

        public static GlobalSetting Instance
        {
            get { return _instance; }
        }

        public string BaseEndpoint
        {
            get { return _baseEndpoint; }
            set
            {
                _baseEndpoint = value;
                UpdateEndpoint(_baseEndpoint);
            }
        }

        public string ClientId { get { return "xamarin"; }}

        public string ClientSecret { get { return "secret"; }}

        public string AuthToken { get; set; }

        public string RegisterWebsite { get; set; }

        public string IdentityEndpoint { get; set; }

        public string UserInfoEndpoint { get; set; }

        public string TokenEndpoint { get; set; }

        public string LogoutEndpoint { get; set; }

        public string IdentityCallback { get; set; }

        public string LogoutCallback { get; set; }

        private void UpdateEndpoint(string baseEndpoint)
        {
            var identityBaseEndpoint = $"{baseEndpoint}/identity";
            RegisterWebsite = $"{identityBaseEndpoint}/Account/Register";
            LogoutCallback = $"{identityBaseEndpoint}/Account/Redirecting";

            var connectBaseEndpoint = $"{identityBaseEndpoint}/connect";
            IdentityEndpoint = $"{connectBaseEndpoint}/authorize";
            UserInfoEndpoint = $"{connectBaseEndpoint}/userinfo";
            TokenEndpoint = $"{connectBaseEndpoint}/token";
            LogoutEndpoint = $"{connectBaseEndpoint}/endsession";
			
            IdentityCallback = $"{baseEndpoint}/xamarincallback";
        }
    }
}