using ApplicationCore.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Identity
{
    public class AppIdentityDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserAddress> UserAddresses { get; set; }
        public DbSet<AuthConfig> AuthConfigs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            builder.Entity<UserAddress>().ToTable("UserAddress");
            builder.Entity<UserAddress>().HasOne(x => x.User)
                .WithMany(u => u.Addresses)
                .HasForeignKey(x => x.UserId)
                .IsRequired();

            builder.Entity<UserAddress>().Property(x => x.Name)
                .HasMaxLength(255);
            builder.Entity<UserAddress>().Property(x => x.ContactNumber)
                .HasMaxLength(255);

            builder.Entity<AuthConfig>().ToTable("AuthConfig");

            builder.Entity<AuthConfig>().Property(x => x.AccessToken)
                .HasMaxLength(255);

            builder.Entity<AuthConfig>().Property(x => x.RefreshToken)
                .HasMaxLength(255);

            builder.Entity<AuthConfig>().Property(x => x.ClientId)
                .HasMaxLength(255);

            builder.Entity<AuthConfig>().Property(x => x.ClientSecret)
                .HasMaxLength(255);

            builder.Entity<AuthConfig>().Property(x => x.SigningSecret)
                .HasMaxLength(255);

            builder.Entity<AuthConfig>().Property(x => x.CallbackURL)
                .HasMaxLength(255);
        }
    }

}
