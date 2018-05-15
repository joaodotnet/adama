using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dama.API.Extensions
{
    public static class CatalogItemExtensions
    {
        public static void FillProductUrl(this CatalogItem item, string picBaseUrl, bool azureStorageEnabled)
        {
            //item.PictureUri = azureStorageEnabled
            //       ? picBaseUrl + item.PictureFileName
            //       : picBaseUrl.Replace("[0]", item.Id.ToString());
        }
    }
}
