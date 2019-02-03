using ApplicationCore.Entities;
using ApplicationCore.Entities.BasketAggregate;
using ApplicationCore.Entities.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Infrastructure.Data
{
    public class GroceryContext : DbContext
    {
        public GroceryContext(DbContextOptions<GroceryContext> options) : base(options)
        {

        }

        public DbSet<Basket> Baskets { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }
        public DbSet<CatalogItem> CatalogItems { get; set; }        
        public DbSet<CatalogType> CatalogTypes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ApplicationCore.Entities.OrderAggregate.Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<CatalogCategory> CatalogCategories { get; set; }
        public DbSet<Country> Countries { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Basket>(ConfigureBasket);
            builder.Entity<BasketItem>(ConfigureBasketItem);
            builder.Entity<Category>(ConfigureCategory);
            builder.Entity<CatalogType>(ConfigureCatalogType);            
            builder.Entity<CatalogItem>(ConfigureCatalogItem);
            builder.Entity<CatalogCategory>(ConfigureCatalogCategories);
            builder.Entity<Order>(ConfigureOrder);
            builder.Entity<OrderItem>(ConfigureOrderItem);
            builder.Entity<Country>(ConfigureCountries);
        }

        private void ConfigureCountries(EntityTypeBuilder<Country> builder)
        {
            builder.ToTable("Country");

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.Code)
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();
        }

        private void ConfigureBasket(EntityTypeBuilder<Basket> builder)
        {
            builder.ToTable("Basket");
            var navigation = builder.Metadata.FindNavigation(nameof(Basket.Items));

            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }

        private void ConfigureBasketItem(EntityTypeBuilder<BasketItem> builder)
        {
            builder.ToTable("BasketItem");
            builder.Property(x => x.CustomizeName)
                .HasMaxLength(100);
            builder.Property(x => x.CustomizeSide)
                .HasMaxLength(100);

            builder.HasOne(bi => bi.Basket)
                .WithMany(b => b.Items)
                .HasForeignKey(bi => bi.BasketId);
            builder.Property(x => x.UnitPrice)
                .HasColumnType("decimal(18,2)");

            //builder.Ignore(x => x.CustomizeName);
            builder.Ignore(x => x.CustomizeSide);
        }

        private void ConfigureCategory(EntityTypeBuilder<Category> builder)
        {
            //Category
            builder.ToTable("Category");
            builder.Ignore(b => b.Position);
            builder.Ignore(c => c.CatalogTypes);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(c => c.Order)
                .IsRequired()
                .HasDefaultValue(1);
            
            builder.HasOne(x => x.Parent)
                .WithMany()
                .HasForeignKey(x => x.ParentId);
            builder
                .HasIndex(c => c.Name)
                .IsUnique();
        }
        private void ConfigureCatalogType(EntityTypeBuilder<CatalogType> builder)
        {
            builder.ToTable("CatalogType");

            builder.HasKey(ci => ci.Id);

            builder.Property(ci => ci.Id)
               //.ForSqlServerUseSequenceHiLo("catalog_type_hilo")
               .IsRequired();

            builder.Property(cb => cb.Code)
                .IsRequired()
                .HasMaxLength(25);

            builder.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.HasIndex(x => x.Code)
               .IsUnique();

            builder.Ignore(x => x.AdditionalTextPrice);
            builder.Ignore(x => x.ShippingCost);
            builder.Ignore(x => x.PictureUri);
            builder.Ignore(x => x.DeliveryTimeMin);
            builder.Ignore(x => x.DeliveryTimeMax);
            builder.Ignore(x => x.DeliveryTimeUnit);
            builder.Ignore(x => x.Categories);
            builder.Ignore(x => x.PictureTextHelpers);
        }

        private void ConfigureCatalogItem(EntityTypeBuilder<CatalogItem> builder)
        {
            builder.ToTable("Catalog");

            builder.Property(ci => ci.Sku)
                .HasMaxLength(255);

            builder.Property(ci => ci.Id)
                //.ForSqlServerUseSequenceHiLo("catalog_hilo")
                .IsRequired();

            builder.Property(ci => ci.Name)
                .IsRequired(true)
                .HasMaxLength(100);

            builder.Property(ci => ci.Price)
                .HasColumnType("decimal(18,2)");
            //    .IsRequired(true);
            
            builder.Ignore(ci => ci.CatalogIllustration);
            builder.Ignore(ci => ci.CatalogIllustrationId);

            builder.HasOne(ci => ci.CatalogType)
                .WithMany(x => x.CatalogItems)
                .HasForeignKey(ci => ci.CatalogTypeId);

            builder.Property(x => x.ShowOnShop)
                .IsRequired(true);
            builder.Ignore(x => x.IsNew);
            builder.Ignore(x => x.IsFeatured);
            builder.Ignore(x => x.CanCustomize);
            builder.Ignore(x => x.Description);
            builder.Ignore(ci => ci.CatalogReferences);
            builder.Ignore(ci => ci.CatalogPictures);             
        }

        private void ConfigureCatalogCategories(EntityTypeBuilder<CatalogCategory> builder)
        {
            builder.ToTable("CatalogCategory");

            builder.Ignore(x => x.Id);
            builder.HasKey(x => new { x.CatalogItemId, x.CategoryId });

            builder.HasOne(x => x.CatalogItem)
              .WithMany(p => p.CatalogCategories)
              .HasForeignKey(x => x.CatalogItemId);

            builder.HasOne(x => x.Category)
               .WithMany(p => p.CatalogCategories)
               .HasForeignKey(x => x.CategoryId);
        }

        private void ConfigureOrder(EntityTypeBuilder<Order> builder)
        {
            builder.Property(x => x.OrderState)
                .IsRequired()
                .HasDefaultValue(OrderStateType.PENDING);
            
            builder.Property(x => x.SalesInvoiceNumber)
                .HasMaxLength(255);

            var navigation = builder.Metadata.FindNavigation(nameof(Order.OrderItems));

            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.Ignore(o => o.ShipToAddress);
            builder.OwnsOne(o => o.BillingToAddress);

            builder.Ignore(x => x.PhoneNumber);
            builder.Ignore(x => x.ShippingCost);
            builder.Ignore(x => x.SalesPaymentId);
            builder.Ignore(x => x.UseBillingSameAsShipping);
        }

        private void ConfigureOrderItem(EntityTypeBuilder<OrderItem> builder)
        {
            builder.OwnsOne(i => i.ItemOrdered);

            builder.Ignore(x => x.CustomizeName);
            builder.Ignore(x => x.CustomizeSide);
            builder.Ignore(x => x.CustomizeItem);
            

            builder.Property(x => x.UnitPrice)
                .HasColumnType("decimal(18,2)");
        }


    }
}
