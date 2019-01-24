using ApplicationCore.Entities;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class GroceryContextSeed
    {
        public static async Task SeedAsync(GroceryContext context)
        {

            if (!context.CatalogItems.Any(x => x.Id == -1))
            {
                context.CatalogItems.Add(new CatalogItem
                {
                    Id = -1,
                    Name = "Produto Personalizado",
                    ShowOnShop = false,
                    CatalogType = context.CatalogTypes.First()
                });

                await context.SaveChangesAsync();
            }
        }

    }
}
