using ApplicationCore.Interfaces;

namespace ApplicationCore.Services
{
    public class UriComposer : IUriComposer
    {
        private readonly CatalogSettings _catalogSettings;

        public UriComposer(CatalogSettings catalogSettings) => _catalogSettings = catalogSettings;

        public string ComposePicUri(string uriTemplate)
        {
            if(!string.IsNullOrEmpty(uriTemplate))
                return uriTemplate.Replace("http://localhost:5500/images/", _catalogSettings.CatalogBaseUrl);
            return "";
        }
    }
}
