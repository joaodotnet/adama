using ApplicationCore;
using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

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
            if (!catalogContext.ShopConfigs.Any(x => x.Type == ShopConfigType.SEO))
            {
                catalogContext.ShopConfigs.AddRange(
                    new ShopConfig
                    {
                        Type = ShopConfigType.SEO,
                        IsActive = true,
                        Name = "Meta Description",
                        Value = "Bem-vindo à Dama no Jornal®. A Loja Online que oferece os Presentes Personalizados mais Criativos. Vem conhecer os nossos Produtos e Personaliza!"
                    },
                    new ShopConfig
                    {
                        Type = ShopConfigType.SEO,
                        IsActive = true,
                        Name = "Title",
                        Value = "Dama no Jornal - Loja Online"
                    });
                await catalogContext.SaveChangesAsync();
            }

            if(catalogContext.CatalogItems.All(x => string.IsNullOrEmpty(x.Slug)))
            {
                //Fix Duplicates catalog Names
                var duplicates = await catalogContext.CatalogItems
                    .GroupBy(x => x.Name)
                    .Where(g => g.Count() > 1)
                    .ToListAsync();
                foreach (var item in duplicates)
                {
                    for (int i = 0; i < item.Count(); i++)
                    {
                        item.ElementAt(i).Name = $"{item.ElementAt(i).Name}-{i + 1}";
                    }
                    
                }

                await catalogContext.SaveChangesAsync();

                //Update Slug
                await catalogContext.CatalogItems.ForEachAsync(c => c.Slug = Utils.URLFriendly(c.Name));

                await catalogContext.SaveChangesAsync();
            }

            if(catalogContext.Categories.All(x => string.IsNullOrEmpty(x.Slug)))
            {
                await catalogContext.Categories.ForEachAsync(x => x.Slug = Utils.URLFriendly(x.Name));
                await catalogContext.SaveChangesAsync();
            }

            if(catalogContext.CatalogTypes.All(x => string.IsNullOrEmpty(x.Slug)))
            {
                var bolsas = await catalogContext.CatalogTypes.Where(x => x.Description == "Bolsa de Telemóvel").ToListAsync();
                if (bolsas?.Count > 1)
                {
                    foreach (var item in bolsas)
                    {
                        item.Description = item.Code == "BTL_M" ? "Bolsa de Telemóvel Mulher" : "Bolsa de Telemóvel Homem";
                    }
                    await catalogContext.SaveChangesAsync();
                }
                await catalogContext.CatalogTypes.ForEachAsync(x => x.Slug = Utils.URLFriendly(x.Description));
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
