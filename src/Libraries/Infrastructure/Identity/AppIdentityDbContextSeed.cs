using ApplicationCore.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

        public static void EnsureDatabaseMigrations(AppIdentityDbContext context)
        {            
            context.Database.Migrate();
        }

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

        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, AppIdentityDbContext context = null)
        {
            //Admin Users
            await CreateUserAsync("joaofbbg@gmail.com", "dama#2018!", null, null, _roleAdmin, _roleGroceryAdmin, _roleStaff);
            await CreateUserAsync("susana.m.mendez@gmail.com", "damasite#2017!", null, null, _roleAdmin, _roleGroceryAdmin, _roleStaff);

            //Seed Staf Users
            await CreateUserAsync("jue@damanojornal.com", "dama#2018!", "João", "Gonçalves", _roleStaff);
            await CreateUserAsync("sue@damanojornal.com", "dama#2018!", "Susana", "Mendez", _roleStaff);
            await CreateUserAsync("sonia@damanojornal.com", "dama#2018!", "Sónia", "Mendez", _roleStaff);
            await CreateUserAsync("rute@saborcomtradicao.com", "mercado#18", "Rute", "Brito", _roleStaff);

            //Sage sales Application
            if (context != null)
            {                
                var salesConfig = await context.AuthConfigs.FindAsync(SageApplicationType.SALESWEB);
                if (salesConfig == null)
                {
                    context.AuthConfigs.Add(new AuthConfig
                    {
                        ApplicationId = SageApplicationType.SALESWEB,
                        ClientId = "a33817c02cd7523e58e8",
                        ClientSecret = "5bf1a4ec81068b81d1ddcd46fe001dc7eefe1bc8",
                        SigningSecret = "9059ae44b05538700be0a402b15bd935092c0ce7",
                        CallbackURL = "http://localhost:6500/Sage/Callback"
                    });
                    await context.SaveChangesAsync();
                }
                var damaConfig = await context.AuthConfigs.FindAsync(SageApplicationType.DAMA_BACKOFFICE);
                if (damaConfig == null)
                {
                    context.AuthConfigs.Add(new AuthConfig
                    {
                        ApplicationId = SageApplicationType.DAMA_BACKOFFICE,
                        ClientId = "3d4d1c18be8c85837d72",
                        ClientSecret = "2b267ffcb846289eff351f405a04c4196010bffb",
                        SigningSecret = "7d84c4209d73e2f7b2f6d0bb7964d4f9e870c1f0",
                        CallbackURL = "http://localhost:5000/Sage/Callback",
                    });
                    await context.SaveChangesAsync();
                }
            }
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
