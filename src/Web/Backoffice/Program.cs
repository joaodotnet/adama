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
using Microsoft.Extensions.Hosting;

namespace Backoffice
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                try
                {
                    var context = services.GetRequiredService<AppIdentityDbContext>();
                    var environment = services.GetRequiredService<IWebHostEnvironment>();
                    AppIdentityDbContextSeed.EnsureDatabaseMigrations(context);
                    await AppIdentityDbContextSeed.EnsureRoleAdminCreated(services);
                    await AppIdentityDbContextSeed.EnsureUserAdminCreated(services, environment.IsDevelopment());
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

                    var damaContext = services.GetRequiredService<DamaContext>();
                    DamaContextSeed.EnsureDatabaseMigrations(damaContext);
                    DamaContextSeed.SeedAsync(damaContext, loggerFactory).Wait();
                }
                catch (Exception ex)
                {
                    var logger = loggerFactory.CreateLogger<Program>();
                    logger.LogError(ex, "An error occurred seeding the DB.");
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
