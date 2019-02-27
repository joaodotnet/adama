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
        public static void EnsureDatabaseMigrations(DamaContext damaContext)
        {            
            damaContext.Database.Migrate();
        }
        public static async Task SeedAsync(DamaContext catalogContext,
            ILoggerFactory loggerFactory, int? retry = 0)
        {
            var seoConfig = await catalogContext.ShopConfigs.SingleOrDefaultAsync(x => x.Type == ShopConfigType.SEO);
            if(seoConfig == null)
            {
                catalogContext.ShopConfigs.Add(new ShopConfig
                {
                    Type = ShopConfigType.SEO,
                    IsActive = true,
                    Name = "Meta Description",
                    Value = "Bem-vindo à Dama no Jornal®. A Loja Online que oferece os Presentes Personalizados mais Criativos. Vem conhecer os nossos Produtos e Personaliza!"
                });
                await catalogContext.SaveChangesAsync();
            }


            //if (!catalogContext.CatalogPrices.Any())
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
            ////TODO: Only run this if using a real database
            // context.Database.Migrate();

            //    if (!catalogContext.CatalogBrands.Any())
            //    {
            //        catalogContext.CatalogBrands.AddRange(
            //            GetPreconfiguredCatalogBrands());

            //        await catalogContext.SaveChangesAsync();
            //    }

            //    if (!catalogContext.CatalogTypes.Any())
            //    {
            //        catalogContext.CatalogTypes.AddRange(
            //            GetPreconfiguredCatalogTypes());

            //        await catalogContext.SaveChangesAsync();
            //    }

            //    if (!catalogContext.CatalogItems.Any())
            //    {
            //        catalogContext.CatalogItems.AddRange(
            //            GetPreconfiguredItems());

            //        await catalogContext.SaveChangesAsync();
            //    }
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

    }
}
