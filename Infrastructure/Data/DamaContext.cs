using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Entities;

namespace Infrastructure.Data
{
    public class DamaContext : DbContext
    {
        public DamaContext(DbContextOptions<DamaContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<Illustration> Illustrations { get; set; }
        public DbSet<ProductAttribute> ProductAttributes { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //Category
            builder.Entity<Category>().ToTable("Category");
            builder.Entity<Category>().Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            //ProductType
            builder.Entity<ProductType>().ToTable("ProductType");
            builder.Entity<ProductType>().Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(25);
            builder.Entity<ProductType>().Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(100);
            builder.Entity<ProductType>().HasOne(x => x.Category)
                .WithMany()
                .HasForeignKey(x => x.CategoryId);

            //Illustration
            builder.Entity<Illustration>().ToTable("Illustration");
            builder.Entity<Illustration>().Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(25);
            builder.Entity<Illustration>().Property(x => x.PictureUri)                
                .HasMaxLength(255);
            builder.Entity<Illustration>().Property(x => x.Type)
                .IsRequired();

            //Product
            builder.Entity<Product>().ToTable("Product");
            builder.Entity<Product>().HasKey(x => x.Id);
            builder.Entity<Product>().Property(x => x.Id)
                .ForSqlServerUseSequenceHiLo("product_hilo")
                .IsRequired();
            builder.Entity<Product>().Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(25);
            builder.Entity<Product>().Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Entity<Product>().Property(x => x.Description)                
                .HasMaxLength(255);
            builder.Entity<Product>().Property(x => x.Personalized)
                .IsRequired()
                .HasDefaultValue(false);
            builder.Entity<Product>().Property(x => x.Price)
                .IsRequired();

            builder.Entity<Product>().HasOne(x => x.Illustation)
                .WithMany()
                .HasForeignKey(x => x.IllustrationId);
            builder.Entity<Product>().HasOne(x => x.ProductType)
                .WithMany()
                .HasForeignKey(x => x.ProductTypeId);

            //ProductAttribute
            builder.Entity<ProductAttribute>().ToTable("ProductAttribute");
            builder.Entity<ProductAttribute>().Property(x => x.Type)
                .IsRequired();
            builder.Entity<ProductAttribute>().Property(x => x.Code)
               .IsRequired()
               .HasMaxLength(25);
            builder.Entity<ProductAttribute>().Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Entity<ProductAttribute>().HasOne(x => x.Product)
                .WithMany(p => p.ProductAttributes)
                .HasForeignKey(x => x.ProductId);

        }
    }
}
