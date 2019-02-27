using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Infrastructure.Identity;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Data;

namespace Backoffice
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                try
                {
                    var context = services.GetRequiredService<AppIdentityDbContext>();
                    AppIdentityDbContextSeed.EnsureDatabaseMigrations(context);
                    AppIdentityDbContextSeed.EnsureRoleAdminCreated(services).Wait();                    
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    AppIdentityDbContextSeed.SeedAsync(userManager, context).Wait();

                    var damaContext = services.GetRequiredService<DamaContext>();
                    DamaContextSeed.EnsureDatabaseMigrations(damaContext);
                    DamaContextSeed.SeedAsync(damaContext, loggerFactory).Wait();

                    var groceryContext = services.GetRequiredService<GroceryContext>();
                    GroceryContextSeed.EnsureDatabaseMigrations(groceryContext);
                }
                catch (Exception ex)
                {
                    var logger = loggerFactory.CreateLogger<Program>();
                    logger.LogError(ex, "An error occurred seeding the DB.");
                }
            }

            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
