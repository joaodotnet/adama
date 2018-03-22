using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Identity
{
    public static class AppIdentityDbContextSeed
    {
        private static string _roleAdmin = "Admin";

        public static async Task EnsureRoleAdminCreated(IServiceProvider services)
        {

            var rolemanager = services.GetRequiredService<RoleManager<IdentityRole>>();

            if (rolemanager.FindByNameAsync(_roleAdmin).Result == null)
            {
                var result = await rolemanager.CreateAsync(new IdentityRole(_roleAdmin));
            }
        }

        public static async Task SeedAsync(UserManager<ApplicationUser> userManager)
        {
            var defaultUser = await userManager.FindByEmailAsync("joaofbbg@gmail.com");
            if (defaultUser == null)
            {
                defaultUser = new ApplicationUser { UserName = "joaofbbg@gmail.com", Email = "joaofbbg@gmail.com" };
                await userManager.CreateAsync(defaultUser, "dama#2017!");
            }

            if(!await userManager.IsInRoleAsync(defaultUser,_roleAdmin))
            {
                await userManager.AddToRoleAsync(defaultUser, _roleAdmin);
            }

            var damaUser = await userManager.FindByEmailAsync("susana.m.mendez@gmail.com");
            if(damaUser == null)
            {
                damaUser = new ApplicationUser { UserName = "susana.m.mendez@gmail.com", Email = "susana.m.mendez@gmail.com" };
                await userManager.CreateAsync(damaUser, "damasite#2017!");
            }

            if (!await userManager.IsInRoleAsync(damaUser, _roleAdmin))
            {
                await userManager.AddToRoleAsync(damaUser, _roleAdmin);
            }

        }
    }
}
