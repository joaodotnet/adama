using DamaNoJornal.Core.Models.Location;
using System.Collections.Generic;

namespace DamaNoJornal.Core
{
    public class GlobalSetting
    {
        public const string AzureTag = "Azure";
        public const string MockTag = "Mock";
        public const string DefaultEndpoint = "http://ec2-35-159-6-83.eu-central-1.compute.amazonaws.com:5859"; //"https://localhost:50390"; // i.e.: "http://YOUR_IP" or "http://YOUR_DNS_NAME"
        public static readonly Place Place1 = new Place { Id = 1, Name = "Feira Popular de Loulé", City = "Loulé", Country = "Portugal", PostalCode = "8100" };
        public static readonly Place Place2 = new Place { Id = 2, Name = "Feira da Serra de São Brás", City = "São Brás", Country = "Portugal", PostalCode = "8151" };
        public static readonly List<Place> Places = new List<Place> { Place1, Place2 };

        private string _baseEndpoint;
        private static readonly GlobalSetting _instance = new GlobalSetting();

        public GlobalSetting()
        {
            AuthToken = "";
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

        public string ClientId { get { return "dama.client"; }}

        public string ClientSecret { get { return "dama.secret"; }}

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
            //var identityBaseEndpoint = $"{baseEndpoint}/identity";
            var identityBaseEndpoint = "http://localhost:5000";
            RegisterWebsite = $"{identityBaseEndpoint}/Account/Register";
            LogoutCallback = $"{identityBaseEndpoint}/Account/Redirecting";

            var connectBaseEndpoint = $"{identityBaseEndpoint}/connect";
            IdentityEndpoint = $"{connectBaseEndpoint}/authorize";
            UserInfoEndpoint = $"{connectBaseEndpoint}/userinfo";
            TokenEndpoint = $"{connectBaseEndpoint}/token";
            LogoutEndpoint = $"{connectBaseEndpoint}/endsession";
			
            IdentityCallback = $"{identityBaseEndpoint}/xamarincallback";
        }
    }
}