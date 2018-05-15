using System.Collections.Generic;
using DamaApp.Core.Models.Basket;
using DamaApp.Core.Models.Catalog;
using DamaApp.Core.Models.Marketing;

namespace DamaApp.Core.Services.FixUri
{
    public interface IFixUriService
    {
        void FixCatalogItemPictureUri(IEnumerable<CatalogItem> catalogItems);
        void FixBasketItemPictureUri(IEnumerable<BasketItem> basketItems);
        void FixCampaignItemPictureUri(IEnumerable<CampaignItem> campaignItems);
    }
}
