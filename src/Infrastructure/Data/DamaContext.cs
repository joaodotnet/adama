using ApplicationCore.Entities.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApplicationCore.Entities;
using System;

namespace Infrastructure.Data
{

    public class DamaContext : DbContext
    {
        public DamaContext(DbContextOptions<DamaContext> options) : base(options)
        {
        }

        public DbSet<Basket> Baskets { get; set; }
        public DbSet<CatalogItem> CatalogItems { get; set; }
        public DbSet<CatalogIllustration> CatalogIllustrations { get; set; }
        public DbSet<IllustrationType> IllustrationTypes { get; set; }
        public DbSet<CatalogType> CatalogTypes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ShopConfig> ShopConfigs { get; set; }
        public DbSet<ApplicationCore.Entities.ShopConfigDetail> ShopConfigDetails { get; set; } //Need the full qualified name for generate code
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Category>(ConfigureCategory);
            builder.Entity<Basket>(ConfigureBasket);
            builder.Entity<CatalogIllustration>(ConfigureCatalogBrand);
            builder.Entity<CatalogType>(ConfigureCatalogType);
            builder.Entity<IllustrationType>(ConfigureIllustrationType);           
            builder.Entity<CatalogItem>(ConfigureCatalogItem);
            builder.Entity<CatalogAttribute>(ConfigureCatalogAttribute);
            builder.Entity<Order>(ConfigureOrder);
            builder.Entity<OrderItem>(ConfigureOrderItem);
            builder.Entity<ShopConfig>(ConfigureShopConfig);
            builder.Entity<ShopConfigDetail>(ConfigureShopConfigDetails);
        }

        private void ConfigureCatalogAttribute(EntityTypeBuilder<CatalogAttribute> builder)
        {
            builder.ToTable("CatalogAttribute");
            builder.Property(x => x.Type)
                .IsRequired();
            builder.Property(x => x.Code)
               .IsRequired()
               .HasMaxLength(25);
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.HasOne(x => x.CatalogItem)
                .WithMany(p => p.CatalogAttributes)
                .HasForeignKey(x => x.CatalogItemId);
        }

        private void ConfigureIllustrationType(EntityTypeBuilder<IllustrationType> builder)
        {
            builder.ToTable("IllustrationType");
            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(25);
            builder.Property(x => x.Name)
                .HasMaxLength(100);
            builder
               .HasIndex(x => x.Code)
               .IsUnique();
        }

        private void ConfigureCategory(EntityTypeBuilder<Category> builder)
        {
            //Category
            builder.ToTable("Category");
            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(c => c.Order)
                .IsRequired()
                .HasDefaultValue(1);
            builder.Property(c => c.Position)
                .IsRequired()
                .HasMaxLength(10)
                .HasDefaultValue("left");
            builder.HasOne(x => x.Parent)
                .WithMany()
                .HasForeignKey(x => x.ParentId);
            builder
                .HasIndex(c => c.Name)
                .IsUnique();
        }

        private void ConfigureBasket(EntityTypeBuilder<Basket> builder)
        {
            var navigation = builder.Metadata.FindNavigation(nameof(Basket.Items));

            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }

        private void ConfigureCatalogItem(EntityTypeBuilder<CatalogItem> builder)
        {
            builder.ToTable("Catalog");

            builder.Property(ci => ci.Id)
                .ForSqlServerUseSequenceHiLo("catalog_hilo")
                .IsRequired();

            builder.Property(ci => ci.Name)
                .IsRequired(true)
                .HasMaxLength(100);

            builder.Property(ci => ci.Price)
                .IsRequired(true);

            builder.Property(ci => ci.PictureUri)
                .IsRequired(false);

            builder.HasOne(ci => ci.CatalogIllustration)
                .WithMany()
                .HasForeignKey(ci => ci.CatalogIllustrationId);

            builder.HasOne(ci => ci.CatalogType)
                .WithMany()
                .HasForeignKey(ci => ci.CatalogTypeId);

            builder.Property(x => x.Description)
                .HasMaxLength(255);
        }

        private void ConfigureCatalogBrand(EntityTypeBuilder<CatalogIllustration> builder)
        {
            builder.ToTable("CatalogIllustration");

            builder.HasKey(ci => ci.Id);

            builder.Property(ci => ci.Id)
               .ForSqlServerUseSequenceHiLo("catalog_brand_hilo")
               .IsRequired();

            builder.Property(cb => cb.Code)
                .IsRequired()
                .HasMaxLength(25);
            
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(x => x.PictureUri)
                .HasMaxLength(255);
            builder.HasOne(x => x.IllustrationType)
                .WithMany()
                .HasForeignKey(x => x.IllustrationTypeId);
            builder
               .HasIndex(x => x.Code)
               .IsUnique();
        }

        private void ConfigureCatalogType(EntityTypeBuilder<CatalogType> builder)
        {
            builder.ToTable("CatalogType");

            builder.HasKey(ci => ci.Id);

            builder.Property(ci => ci.Id)
               .ForSqlServerUseSequenceHiLo("catalog_type_hilo")
               .IsRequired();

            builder.Property(cb => cb.Code)
                .IsRequired()
                .HasMaxLength(25);

            builder.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(100);
            builder.HasOne(x => x.Category)
                .WithMany(c => c.CatalogTypes)
                .HasForeignKey(x => x.CategoryId);
            builder
               .HasIndex(x => x.Code)
               .IsUnique();
        }

        private void ConfigureOrder(EntityTypeBuilder<Order> builder)
        {
            var navigation = builder.Metadata.FindNavigation(nameof(Order.OrderItems));

            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.OwnsOne(o => o.ShipToAddress);
        }

        private void ConfigureOrderItem(EntityTypeBuilder<OrderItem> builder)
        {
            builder.OwnsOne(i => i.ItemOrdered);
        }

        private void ConfigureShopConfig(EntityTypeBuilder<ShopConfig> builder)
        {
            builder.ToTable("ShopConfig");
            builder.Property(x => x.Type)
                .IsRequired();
            builder.Property(x => x.Name)
                .HasMaxLength(100);
            builder.Property(x => x.Value)
               .HasMaxLength(255);
            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
        }
        private void ConfigureShopConfigDetails(EntityTypeBuilder<ShopConfigDetail> builder)
        {
            builder.ToTable("ShopConfigDetail");
            builder.Property(x => x.PictureUri)
                .HasMaxLength(255);
            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.HasOne(x => x.ShopConfig)
                .WithMany(p => p.Details)
                .HasForeignKey(x => x.ShopConfigId);
        }
    }
}
