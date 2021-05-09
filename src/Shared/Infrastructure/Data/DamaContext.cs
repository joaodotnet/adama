using ApplicationCore.Entities.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApplicationCore.Entities;
using System;
using ApplicationCore.Entities.BasketAggregate;

namespace Infrastructure.Data
{

    public class DamaContext : DbContext
    {
        public DamaContext(DbContextOptions<DamaContext> options) : base(options)
        {
        }

        public DbSet<Basket> Baskets { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }
        public DbSet<CatalogItem> CatalogItems { get; set; }
        public DbSet<CatalogIllustration> CatalogIllustrations { get; set; }
        public DbSet<CatalogPicture> CatalogPictures { get; set; }
        public DbSet<IllustrationType> IllustrationTypes { get; set; }
        public DbSet<CatalogType> CatalogTypes { get; set; }
        public DbSet<CatalogTypeCategory> CatalogTypeCategories { get; set; }
        public DbSet<ApplicationCore.Entities.OrderAggregate.Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        //public DbSet<OrderItemDetail> OrderItemDetails { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ShopConfig> ShopConfigs { get; set; }
        public DbSet<ApplicationCore.Entities.ShopConfigDetail> ShopConfigDetails { get; set; } //Need the full qualified name for generate code
        public DbSet<ApplicationCore.Entities.CatalogAttribute> CatalogAttributes { get; set; }
        public DbSet<CatalogCategory> CatalogCategories { get; set; }
        public DbSet<ApplicationCore.Entities.CustomizeOrder> CustomizeOrders { get; set; }
        public DbSet<CatalogReference> CatalogReferences { get; set; }
        public DbSet<FileDetail> FileDetails { get; set; }
        public DbSet<ShippingPriceWeight> ShippingPriceWeights { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Category>(ConfigureCategory);
            builder.Entity<Basket>(ConfigureBasket);
            builder.Entity<BasketItem>(ConfigureBasketItem);
            builder.Entity<CatalogIllustration>(ConfigureCatalogBrand);
            builder.Entity<CatalogType>(ConfigureCatalogType);
            builder.Entity<IllustrationType>(ConfigureIllustrationType);           
            builder.Entity<CatalogItem>(ConfigureCatalogItem);
            builder.Entity<CatalogAttribute>(ConfigureCatalogAttribute);
            builder.Entity<CatalogPicture>(ConfigureCatalogPicture);
            builder.Entity<Order>(ConfigureOrder);
            builder.Entity<OrderItem>(ConfigureOrderItem);
            //builder.Entity<OrderItemDetail>(ConfigureOrderItemDetail);
            builder.Entity<ShopConfig>(ConfigureShopConfig);
            builder.Entity<ShopConfigDetail>(ConfigureShopConfigDetails);
            builder.Entity<CatalogTypeCategory>(ConfigureCatalogTypeCategory);
            builder.Entity<CatalogCategory>(ConfigureCatalogCategories);
            builder.Entity<CustomizeOrder>(ConfigureCustomizeOrders);
            builder.Entity<CatalogReference>(ConfigureCatalogReferences);
            builder.Entity<FileDetail>(ConfigureFileDetails);
        }

        private void ConfigureBasketItem(EntityTypeBuilder<BasketItem> builder)
        {
            builder.ToTable("BasketItem");
            
            builder.Property(x => x.CustomizeSide)
                .HasMaxLength(100);

            builder.HasOne(bi => bi.Basket)
                .WithMany(b => b.Items)
                .HasForeignKey(bi => bi.BasketId);
            builder.Property(x => x.UnitPrice)
                .HasColumnType("decimal(18,2)");
        }

        private void ConfigureFileDetails(EntityTypeBuilder<FileDetail> builder)
        {
            builder.ToTable("FileDetail");
            builder.Property(x => x.PictureUri)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.Location)
                .HasMaxLength(255);
            builder.Property(x => x.FileName)
                .HasMaxLength(100);
            builder.Property(x => x.Extension)
                .HasMaxLength(10);

            builder.HasOne(x => x.CatalogType)
                .WithMany(t => t.PictureTextHelpers)
                .HasForeignKey(x => x.CatalogTypeId);

        }

        private void ConfigureCatalogReferences(EntityTypeBuilder<CatalogReference> builder)
        {
            builder.ToTable("CatalogReference");

            builder.HasOne(x => x.CatalogItem)
               .WithMany(p => p.CatalogReferences)
               .HasForeignKey(x => x.CatalogItemId);

            builder.Property(x => x.LabelDescription)
                .IsRequired()
                .HasMaxLength(20)                
                .HasDefaultValue("Tamanho");

        }

        //private void ConfigureCatalogPrice(EntityTypeBuilder<CatalogPrice> builder)
        //{
        //    builder.ToTable("CatalogPrice");

        //    builder.Property(x => x.Price)
        //        .IsRequired()
        //        .HasColumnType("decimal(18,2)");
        //}

        //private void ConfigureAttributes(EntityTypeBuilder<ApplicationCore.Entities.Attribute> builder)
        //{
        //    builder.ToTable("Attribute");

        //    builder.Property(x => x.Name)
        //        .IsRequired()
        //        .HasMaxLength(100);

        //    builder.Property(x => x.Type)
        //        .IsRequired();
        //}

        private void ConfigureCustomizeOrders(EntityTypeBuilder<CustomizeOrder> builder)
        {
            builder.ToTable("CustomizeOrder");

            builder.Property(x => x.OrderState)
                .IsRequired()
                .HasDefaultValue(OrderStateType.PENDING);
            builder.Property(x => x.BuyerId)
                .IsRequired();
            builder.Property(x => x.BuyerName)
                .IsRequired();
            builder.Property(x => x.BuyerContact)
                .IsRequired();
            builder.Property(x => x.Description)
                .IsRequired();
            builder.OwnsOne(i => i.ItemOrdered);
        }

        private void ConfigureCatalogTypeCategory(EntityTypeBuilder<CatalogTypeCategory> builder)
        {
            builder.ToTable("CatalogTypeCategory");
            builder.HasKey(c => new { c.CatalogTypeId, c.CategoryId });
            builder.HasOne(x => x.CatalogType)
                .WithMany(p => p.Categories)
                .HasForeignKey(x => x.CatalogTypeId);

            builder.HasOne(x => x.Category)
                .WithMany(p => p.CatalogTypes)
                .HasForeignKey(x => x.CategoryId);
        }

        private void ConfigureCatalogPicture(EntityTypeBuilder<CatalogPicture> builder)
        {
            builder.ToTable("CatalogPicture");
            builder.Property(x => x.PictureUri)
                .HasMaxLength(1000)
                .IsRequired();
            builder.Property(x => x.PictureHighUri)
                .HasMaxLength(1000);
            builder.Property(x => x.PictureLowUri)
                .HasMaxLength(1000);
            builder.HasOne(x => x.CatalogItem)
                .WithMany(p => p.CatalogPictures)
                .HasForeignKey(x => x.CatalogItemId);
        }

        private void ConfigureCatalogAttribute(EntityTypeBuilder<CatalogAttribute> builder)
        {
            builder.ToTable("CatalogAttribute");
            //builder.HasIndex(x => x.Sku)
            //    .IsUnique();
            //builder.Property(x => x.Sku)
            //    .HasMaxLength(255);
            //builder.HasOne(x => x.Attribute)
            //    .WithMany(a => a.CatalogAttributes)
            //    .HasForeignKey(x => x.AttributeId)
            //    .IsRequired();
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.HasOne(x => x.CatalogItem)
                .WithMany(p => p.Attributes)
                .HasForeignKey(x => x.CatalogItemId);
            //builder.HasOne(x => x.ReferenceCatalogItem)
            //    .WithMany(x => x.ReferenceCatalogAttributes)
            //    .HasForeignKey(x => x.ReferenceCatalogItemId);
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
            builder.Property(c => c.MetaDescription)
                .HasMaxLength(161);
            builder.Property(c => c.Title)
                .HasMaxLength(61);
            builder.HasOne(x => x.Parent)
                .WithMany()
                .HasForeignKey(x => x.ParentId);
            builder.HasIndex(c => c.Name)
                .IsUnique();
            builder.HasIndex(x => x.Slug)
                .IsUnique();
            builder.Property(x => x.Slug)
                .HasMaxLength(100);
            builder.Property(x => x.H1Text)
               .HasMaxLength(50);
        }

        private void ConfigureBasket(EntityTypeBuilder<Basket> builder)
        {           
            var navigation = builder.Metadata.FindNavigation(nameof(Basket.Items));

            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.Property(x => x.Observations)
                .HasMaxLength(5000);

            builder.Property(x => x.IsGuest)
                .HasDefaultValue(false);

        }

        private void ConfigureCatalogItem(EntityTypeBuilder<CatalogItem> builder)
        {
            builder.ToTable("Catalog");

            //builder.HasIndex(ci => ci.Sku)
            //    .IsUnique();            
            builder.Property(ci => ci.Sku)
                .HasMaxLength(255);

            builder.HasIndex(ci => ci.Slug)
                .IsUnique();
            builder.Property(ci => ci.Slug)
                .HasMaxLength(100);

            builder.Property(ci => ci.Id)
                //.ForSqlServerUseSequenceHiLo("catalog_hilo")
                .IsRequired();

            builder.Property(ci => ci.Name)
                .IsRequired(true)
                .HasMaxLength(100);

            builder.Property(ci => ci.Price)
                .HasColumnType("decimal(18,2)");

            builder.Property(ci => ci.Discount)
                .HasColumnType("decimal(18,2)");

            builder.HasOne(ci => ci.CatalogIllustration)
                .WithMany()
                .HasForeignKey(ci => ci.CatalogIllustrationId);

            builder.HasOne(ci => ci.CatalogType)
                .WithMany(x => x.CatalogItems)
                .HasForeignKey(ci => ci.CatalogTypeId);

            builder.Property(x => x.ShowOnShop)
                .IsRequired(true);
            builder.Property(x => x.IsNew)
                .IsRequired(true);
            builder.Property(x => x.IsFeatured)
                .IsRequired(true);
            builder.Property(x => x.CanCustomize)
                .IsRequired(true);
            //.HasDefaultValue(false);

            builder.Property(c => c.MetaDescription)
                .HasMaxLength(161);
            builder.Property(c => c.Title)
                .HasMaxLength(61);
        }

        private void ConfigureCatalogBrand(EntityTypeBuilder<CatalogIllustration> builder)
        {
            builder.ToTable("CatalogIllustration");

            builder.HasKey(ci => ci.Id);

            builder.Property(ci => ci.Id)
               //.ForSqlServerUseSequenceHiLo("catalog_brand_hilo")
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
               //.ForSqlServerUseSequenceHiLo("catalog_type_hilo")
               .IsRequired();

            builder.Property(cb => cb.Code)
                .IsRequired()
                .HasMaxLength(25);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Price)
                .IsRequired()                
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.AdditionalTextPrice)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.DeliveryTimeMin)
                .IsRequired()
                .HasDefaultValue(2);
            builder.Property(x => x.DeliveryTimeMax)
                .IsRequired()
                .HasDefaultValue(3);
            builder.Property(x => x.DeliveryTimeUnit)
                .IsRequired()
                .HasDefaultValue(DeliveryTimeUnitType.Days);

            builder.Property(c => c.MetaDescription)
                .HasMaxLength(161);
            builder.Property(c => c.Title)
                .HasMaxLength(61);

            builder.HasIndex(x => x.Code)
               .IsUnique();
            builder.Property(x => x.PictureUri)
                .HasMaxLength(255);

            builder.HasIndex(x => x.Slug)
                .IsUnique();
            builder.Property(x => x.Slug)
                .HasMaxLength(100);

            builder.Property(x => x.H1Text)
                .HasMaxLength(50);

            builder.Property(x => x.Question)
                .HasMaxLength(255);
        }

        private void ConfigureOrder(EntityTypeBuilder<Order> builder)
        {
            builder.Property(x => x.OrderState)
                .IsRequired()
                .HasDefaultValue(OrderStateType.PENDING);

            builder.Property(x => x.PhoneNumber)
                .HasMaxLength(100);

            builder.Property(x => x.SalesInvoiceNumber)
                .HasMaxLength(255);

            builder.Property(x => x.ShippingCost)
                 .HasColumnType("decimal(18,2)");

            var navigation = builder.Metadata.FindNavigation(nameof(Order.OrderItems));

            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.Property(x => x.Observations)
                .HasMaxLength(5000);

            builder.OwnsOne(o => o.ShipToAddress);
            builder.OwnsOne(o => o.BillingToAddress);
            builder.Ignore(o => o.CustomerEmail);
        }

        private void ConfigureOrderItem(EntityTypeBuilder<OrderItem> builder)
        {
            builder.OwnsOne(i => i.ItemOrdered);

            builder.OwnsOne(i => i.CustomizeItem);
        
            builder.Property(x => x.CustomizeSide)
                .HasMaxLength(100);
            builder.Property(x => x.UnitPrice)
                .HasColumnType("decimal(18,2)");
        }

        //private void ConfigureOrderItemDetail(EntityTypeBuilder<OrderItemDetail> builder)
        //{            
        //    builder.Property(x => x.AttributeName)
        //        .IsRequired()
        //        .HasMaxLength(100);

        //    builder.HasOne(x => x.OrderItem)
        //        .WithMany(o => o.Details)
        //        .HasForeignKey(x => x.OrderItemId);
        //}

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
                .IsRequired();
        }
        private void ConfigureShopConfigDetails(EntityTypeBuilder<ShopConfigDetail> builder)
        {
            builder.ToTable("ShopConfigDetail");
            builder.Property(x => x.PictureUri)
                .HasMaxLength(255);
            builder.Property(x => x.PictureWebpUri)
                .HasMaxLength(255);
            builder.Property(x => x.PictureMobileUri)
                .HasMaxLength(255);
            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.HasOne(x => x.ShopConfig)
                .WithMany(p => p.Details)
                .HasForeignKey(x => x.ShopConfigId);
        }
        private void ConfigureCatalogCategories(EntityTypeBuilder<CatalogCategory> builder)
        {
            builder.ToTable("CatalogCategory");

            builder.HasOne(x => x.Category)
               .WithMany(p => p.CatalogCategories)
               .HasForeignKey(x => x.CategoryId);
        }
    }
}
