using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class DamaContextSeed
    {
        private static readonly Regex reSlugCharactersToBeDashes = new Regex(@"([\s,.//\\-_=])+");
        private static readonly Regex reSlugCharactersToRemove = new Regex(@"([^0-9a-z\-])+");
        private static readonly Regex reSlugDashes = new Regex(@"([\-])+");
        private static readonly Regex reSlugCharacters = new Regex(@"([\s,.//\\-_=])+");

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
                //Fix Individual - The Secret Life of Pets Title
                var pets = await catalogContext.CatalogItems.Where(x => x.Name == "Individual - The Secret Life of Pets Title").ToListAsync();
                for (int i = 0; i < pets.Count; i++)
                {
                    pets[i].Name = $"{pets[i].Name}-{i+1}";
                }

                await catalogContext.SaveChangesAsync();

                //Update Slug
                await catalogContext.CatalogItems.ForEachAsync(c => c.Slug = URLFriendly(c.Name));

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

        private static string URLFriendly(string title)
        {
            if (string.IsNullOrEmpty(title)) return "";

            var newTitle = RemoveDiacritics(title);

            newTitle = Regex.Replace(newTitle, "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])", @"-$1");

            newTitle = reSlugCharactersToBeDashes.Replace(newTitle, "-");

            newTitle = newTitle.ToLowerInvariant();

            newTitle = reSlugCharactersToRemove.Replace(newTitle, "");

            newTitle = reSlugDashes.Replace(newTitle, "-");

            newTitle = newTitle.Trim('-');

            return newTitle;
        }

        static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

    }
}
