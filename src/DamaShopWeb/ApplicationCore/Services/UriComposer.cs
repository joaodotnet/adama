using ApplicationCore.Interfaces;
using DamaShopWeb;

namespace ApplicationCore.Services
{
    public class UriComposer : IUriComposer
    {
        private readonly CatalogSettings _catalogSettings;

        public UriComposer(CatalogSettings catalogSettings) => _catalogSettings = catalogSettings;

        public string ComposePicUri(string uriTemplate)
        {
            return uriTemplate.Replace("https://www.damanojornal.com/loja/images/", _catalogSettings.CatalogBaseUrl);
        }
    }
}
