using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Identity
{
    public static class AppIdentityDbContextSeed
    {
        private static string _roleAdmin = "Admin";
        private static string _roleGroceryAdmin = "GroceryAdmin";
        private static string _roleStaff = "Staff";

        public static async Task EnsureRoleAdminCreated(IServiceProvider services)
        {

            var rolemanager = services.GetRequiredService<RoleManager<IdentityRole>>();

            if (rolemanager.FindByNameAsync(_roleAdmin).Result == null)
            {
                var result = await rolemanager.CreateAsync(new IdentityRole(_roleAdmin));
            }

            if (rolemanager.FindByNameAsync(_roleStaff).Result == null)
            {
                var result = await rolemanager.CreateAsync(new IdentityRole(_roleStaff));
            }
            if (rolemanager.FindByNameAsync(_roleGroceryAdmin).Result == null)
            {
                var result = await rolemanager.CreateAsync(new IdentityRole(_roleGroceryAdmin));
            }


        }

        public static async Task SeedAsync(UserManager<ApplicationUser> userManager)
        {
            //Admin Users
            await CreateUserAsync("joaofbbg@gmail.com", "dama#2018!", null, null, _roleAdmin, _roleGroceryAdmin);
            await CreateUserAsync("susana.m.mendez@gmail.com", "damasite#2017!", null, null, _roleAdmin, _roleGroceryAdmin);

            //Seed Staf Users
            await CreateUserAsync("jue@damanojornal.com", "dama#2018!", "João", "Gonçalves", _roleStaff);
            await CreateUserAsync("sue@damanojornal.com", "dama#2018!", "Susana", "Mendez", _roleStaff);
            await CreateUserAsync("sonia@damanojornal.com", "dama#2018!", "Sónia", "Mendez", _roleStaff);

            //local Function
            async Task CreateUserAsync(string email, string password, string firstName, string lastName, params string[] roles)
            {
                var defaultUser = await userManager.FindByEmailAsync(email);
                if (defaultUser == null)
                {
                    defaultUser = new ApplicationUser { UserName = email, Email = email, FirstName = firstName, LastName = lastName };
                    await userManager.CreateAsync(defaultUser, password);
                }

                foreach (var role in roles)
                {
                    if (!await userManager.IsInRoleAsync(defaultUser, role))
                    {
                        await userManager.AddToRoleAsync(defaultUser, role);
                    }
                }
            }
        }
    }
}
