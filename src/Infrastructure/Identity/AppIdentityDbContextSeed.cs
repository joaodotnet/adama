using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Infrastructure.Identity
{
    public class AppIdentityDbContextSeed
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager)
        {
            var defaultUser = new ApplicationUser { UserName = "joaofbbg@gmail.com", Email = "joaofbbg@gmail.com" };
            await userManager.CreateAsync(defaultUser, "dama#2017!");

            var damaUser = new ApplicationUser { UserName = "susana.m.mendez@gmail.com", Email = "susana.m.mendez@gmail.com" };
            await userManager.CreateAsync(damaUser, "damasite#2017!");
        }
    }
}
