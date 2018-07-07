using Microsoft.AspNetCore.Builder;
using ApplicationCore.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class DamaContextSeed
    {
        public static async Task SeedAsync(DamaContext catalogContext,
            ILoggerFactory loggerFactory, int? retry = 0)
        {
            //if(!catalogContext.CatalogPrices.Any())
            //{
            //    var products = await catalogContext.CatalogItems
            //        .Include(x => x.CatalogType)
            //        .ToListAsync();                
            //    foreach (var item in products)
            //    {
            //        catalogContext.CatalogPrices.Add(new CatalogPrice
            //        {
            //            CatalogItemId = item.Id,
            //            Price = item.Price ?? item.CatalogType.Price,
            //            Active = true,
            //        });
            //    }
            //    await catalogContext.SaveChangesAsync();
            //}
            //int retryForAvailability = retry.Value;
            //try
            //{
                // TODO: Only run this if using a real database
                // context.Database.Migrate();

                //if (!catalogContext.CatalogBrands.Any())
                //{
                //    catalogContext.CatalogBrands.AddRange(
                //        GetPreconfiguredCatalogBrands());

                //    await catalogContext.SaveChangesAsync();
                //}

                //if (!catalogContext.CatalogTypes.Any())
                //{
                //    catalogContext.CatalogTypes.AddRange(
                //        GetPreconfiguredCatalogTypes());

                //    await catalogContext.SaveChangesAsync();
                //}

                //if (!catalogContext.CatalogItems.Any())
                //{
                //    catalogContext.CatalogItems.AddRange(
                //        GetPreconfiguredItems());

                //    await catalogContext.SaveChangesAsync();
                //}
            //}
            //catch (Exception ex)
            //{
            //    if (retryForAvailability < 10)
            //    {
            //        retryForAvailability++;
            //        var log = loggerFactory.CreateLogger<DamaContextSeed>();
            //        log.LogError(ex.Message);
            //        await SeedAsync(catalogContext, loggerFactory, retryForAvailability);
            //    }
            //}
        }

        static IEnumerable<CatalogIllustration> GetPreconfiguredCatalogBrands()
        {
            return new List<CatalogIllustration>()
            {
                new CatalogIllustration() { Code = "Azure"},
                new CatalogIllustration() { Code = ".NET" },
                new CatalogIllustration() { Code = "Visual Studio" },
                new CatalogIllustration() { Code = "SQL Server" }, 
                new CatalogIllustration() { Code = "Other" }
            };
        }

        static IEnumerable<CatalogType> GetPreconfiguredCatalogTypes()
        {
            return new List<CatalogType>()
            {
                new CatalogType() { Code = "Mug"},
                new CatalogType() { Code = "T-Shirt" },
                new CatalogType() { Code = "Sheet" },
                new CatalogType() { Code = "USB Memory Stick" }
            };
        }

        static IEnumerable<CatalogItem> GetPreconfiguredItems()
        {
            return new List<CatalogItem>()
            {
                new CatalogItem() { CatalogTypeId=2,CatalogIllustrationId=2, Description = ".NET Bot Black Sweatshirt", Name = ".NET Bot Black Sweatshirt", Price = 19.5M, PictureUri = "https://www.damanojornal.com/loja/images/products/1.png" },
                new CatalogItem() { CatalogTypeId=1,CatalogIllustrationId=2, Description = ".NET Black & White Mug", Name = ".NET Black & White Mug", Price= 8.50M, PictureUri = "https://www.damanojornal.com/loja/images/products/2.png" },
                new CatalogItem() { CatalogTypeId=2,CatalogIllustrationId=5, Description = "Prism White T-Shirt", Name = "Prism White T-Shirt", Price = 12, PictureUri = "https://www.damanojornal.com/loja/images/products/3.png" },
                new CatalogItem() { CatalogTypeId=2,CatalogIllustrationId=2, Description = ".NET Foundation Sweatshirt", Name = ".NET Foundation Sweatshirt", Price = 12, PictureUri = "https://www.damanojornal.com/loja/images/products/4.png" },
                new CatalogItem() { CatalogTypeId=3,CatalogIllustrationId=5, Description = "Roslyn Red Sheet", Name = "Roslyn Red Sheet", Price = 8.5M, PictureUri = "https://www.damanojornal.com/loja/images/products/5.png" },
                new CatalogItem() { CatalogTypeId=2,CatalogIllustrationId=2, Description = ".NET Blue Sweatshirt", Name = ".NET Blue Sweatshirt", Price = 12, PictureUri = "https://www.damanojornal.com/loja/images/products/6.png" },
                new CatalogItem() { CatalogTypeId=2,CatalogIllustrationId=5, Description = "Roslyn Red T-Shirt", Name = "Roslyn Red T-Shirt", Price = 12, PictureUri = "https://www.damanojornal.com/loja/images/products/7.png"  },
                new CatalogItem() { CatalogTypeId=2,CatalogIllustrationId=5, Description = "Kudu Purple Sweatshirt", Name = "Kudu Purple Sweatshirt", Price = 8.5M, PictureUri = "https://www.damanojornal.com/loja/images/products/8.png" },
                new CatalogItem() { CatalogTypeId=1,CatalogIllustrationId=5, Description = "Cup<T> White Mug", Name = "Cup<T> White Mug", Price = 12, PictureUri = "https://www.damanojornal.com/loja/images/products/9.png" },
                new CatalogItem() { CatalogTypeId=3,CatalogIllustrationId=2, Description = ".NET Foundation Sheet", Name = ".NET Foundation Sheet", Price = 12, PictureUri = "https://www.damanojornal.com/loja/images/products/10.png" },
                new CatalogItem() { CatalogTypeId=3,CatalogIllustrationId=2, Description = "Cup<T> Sheet", Name = "Cup<T> Sheet", Price = 8.5M, PictureUri = "https://www.damanojornal.com/loja/images/products/11.png" },
                new CatalogItem() { CatalogTypeId=2,CatalogIllustrationId=5, Description = "Prism White TShirt", Name = "Prism White TShirt", Price = 12, PictureUri = "https://www.damanojornal.com/loja/images/products/12.png" }
            };
        }
    }
}
