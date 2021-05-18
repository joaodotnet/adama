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
            ILoggerFactory loggerFactory, int? retry = 0, bool isDevelopment = false)
        {
            var log = loggerFactory.CreateLogger<DamaContextSeed>();
            if(isDevelopment)
            {
                if(!catalogContext.Categories.Any())
                {
                    var cat1 = await catalogContext.Categories.AddAsync(new Category("Acessórios","acessorios",1));
                    var cat2 = await catalogContext.Categories.AddAsync(new Category("Papelaria","papelaria",2));
                    var cat3 = await catalogContext.Categories.AddAsync(new Category("Decoração","decoracao",3));

                    await catalogContext.SaveChangesAsync();
                
                    var type1 = new CatalogType("Canecas","http://localhost:5500/images/producttypes/canecas-20.jpg");
                    type1.AddCategory(new CatalogTypeCategory(cat1.Entity.Id));
                    await catalogContext.CatalogTypes.AddAsync(type1);
                    var type2 = new CatalogType("Cadernos","http://localhost:5500/images/producttypes/cadernos-11.jpg");
                    type2.AddCategory(new CatalogTypeCategory(cat2.Entity.Id));
                    await catalogContext.CatalogTypes.AddAsync(type2);
                    var type3 = new CatalogType("Imãs", "http://localhost:5500/images/producttypes/imas-38.jpg");
                    type3.AddCategory(new CatalogTypeCategory(cat3.Entity.Id));
                    await catalogContext.CatalogTypes.AddAsync(type3);

                    await catalogContext.SaveChangesAsync();

                    var illustrationType = new IllustrationType("Ferias","Ferias");
                    await catalogContext.IllustrationTypes.AddAsync(illustrationType);
                    await catalogContext.SaveChangesAsync();

                    var illustration = new CatalogIllustration("DAMA_FERIAS", "Dama Férias", illustrationType.Id);
                    await catalogContext.CatalogIllustrations.AddAsync(illustration);

                    await catalogContext.SaveChangesAsync();
                    
                    var item1 = new CatalogItem("Carteira de Mulher","carteira-de-mulher", 18.99m, "http://localhost:5500/images/products/dama-de-ferias-(2)-75.jpg", true, true, true, true, illustration.Id, type1.Id);
                    item1.AddCategory(cat1.Entity.Id);
                    await catalogContext.CatalogItems.AddAsync(item1);

                    var item2 = new CatalogItem("Caderno Educadora","caderno-educadora", 9.99m, "http://localhost:5500/images/products/cadernos-(1)-208.jpg", true, true, true, true, illustration.Id, type2.Id);
                    item2.AddCategory(cat2.Entity.Id);
                    await catalogContext.CatalogItems.AddAsync(item2);

                    var item3 = new CatalogItem("Ima Auxiliar","ima-auxiliar", 5.99m, "http://localhost:5500/images/products/iman-auxiliar-264.jpg", true, true, true, true, illustration.Id, type3.Id);
                    item3.AddCategory(cat3.Entity.Id);
                    await catalogContext.CatalogItems.AddAsync(item3);

                    await catalogContext.SaveChangesAsync();

                    log.LogInformation("Seed data create successuful");
                }
            }
            // if(!catalogContext.ShippingPriceWeights.Any())
            // {
            //     catalogContext.ShippingPriceWeights.AddRange(
            //         new ShippingPriceWeight
            //         {
            //             MinWeight = 0,
            //             MaxWeight = 20,
            //             Price = 0.53M
            //         },
            //         new ShippingPriceWeight
            //         {
            //             MinWeight = 20,
            //             MaxWeight = 50,
            //             Price = 0.70M,
            //         },
            //         new ShippingPriceWeight
            //         {
            //             MinWeight = 50,
            //             MaxWeight = 100,
            //             Price = 0.85M,
            //         },
            //         new ShippingPriceWeight
            //         {
            //             MinWeight = 100,
            //             MaxWeight = 500,
            //             Price = 1.50M,
            //         },
            //         new ShippingPriceWeight
            //         {
            //             MinWeight = 500,
            //             MaxWeight = 2000,
            //             Price = 3.5M
            //         });
            //     await catalogContext.SaveChangesAsync();
            // }
            // if (!catalogContext.ShopConfigs.Any(x => x.Type == ShopConfigType.SEO))
            // {
            //     catalogContext.ShopConfigs.AddRange(
            //         new ShopConfig
            //         {
            //             Type = ShopConfigType.SEO,
            //             IsActive = true,
            //             Name = "Meta Description",
            //             Value = "Bem-vindo à Dama no Jornal®. A Loja Online que oferece os Presentes Personalizados mais Criativos. Vem conhecer os nossos Produtos e Personaliza!"
            //         },
            //         new ShopConfig
            //         {
            //             Type = ShopConfigType.SEO,
            //             IsActive = true,
            //             Name = "Title",
            //             Value = "Dama no Jornal - Loja Online"
            //         });
            //     await catalogContext.SaveChangesAsync();
            // }

            // if(catalogContext.CatalogItems.All(x => string.IsNullOrEmpty(x.Slug)))
            // {
            //     //Fix Duplicates catalog Names
            //     var duplicates = await catalogContext.CatalogItems
            //         .GroupBy(x => x.Name)
            //         .Where(g => g.Count() > 1)
            //         .ToListAsync();
            //     foreach (var item in duplicates)
            //     {
            //         for (int i = 0; i < item.Count(); i++)
            //         {
            //             item.ElementAt(i).UpdateName($"{item.ElementAt(i).Name}-{i + 1}");
            //         }
                    
            //     }

            //     await catalogContext.SaveChangesAsync();

            //     //Update Slug
            //     await catalogContext.CatalogItems.ForEachAsync(c => c.UpdateSlug(Utils.URLFriendly(c.Name)));

            //     await catalogContext.SaveChangesAsync();
            // }

            // if(catalogContext.Categories.All(x => string.IsNullOrEmpty(x.Slug)))
            // {
            //     await catalogContext.Categories.ForEachAsync(x => x.Slug = Utils.URLFriendly(x.Name));
            //     await catalogContext.SaveChangesAsync();
            // }

            // if(catalogContext.CatalogTypes.All(x => string.IsNullOrEmpty(x.Slug)))
            // {
            //     var bolsas = await catalogContext.CatalogTypes.Where(x => x.Name == "Bolsa de Telemóvel").ToListAsync();
            //     if (bolsas?.Count > 1)
            //     {
            //         foreach (var item in bolsas)
            //         {
            //             item.Name = item.Code == "BTL_M" ? "Bolsa de Telemóvel Mulher" : "Bolsa de Telemóvel Homem";
            //         }
            //         await catalogContext.SaveChangesAsync();
            //     }
            //     await catalogContext.CatalogTypes.ForEachAsync(x => x.Slug = Utils.URLFriendly(x.Name));
            //     await catalogContext.SaveChangesAsync();
            // }

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
