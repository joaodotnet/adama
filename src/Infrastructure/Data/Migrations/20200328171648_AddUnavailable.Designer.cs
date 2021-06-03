﻿// <auto-generated />
using System;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Data.Migrations
{
    [DbContext(typeof(DamaContext))]
    [Migration("20200328171648_AddUnavailable")]
    partial class AddUnavailable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("ApplicationCore.Entities.BasketAggregate.Basket", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("BuyerId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("IsGuest")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<string>("Observations")
                        .HasColumnType("longtext CHARACTER SET utf8mb4")
                        .HasMaxLength(5000);

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("Baskets");
                });

            modelBuilder.Entity("ApplicationCore.Entities.BasketAggregate.BasketItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("BasketId")
                        .HasColumnType("int");

                    b.Property<int?>("CatalogAttribute1")
                        .HasColumnType("int");

                    b.Property<int?>("CatalogAttribute2")
                        .HasColumnType("int");

                    b.Property<int?>("CatalogAttribute3")
                        .HasColumnType("int");

                    b.Property<int>("CatalogItemId")
                        .HasColumnType("int");

                    b.Property<int?>("CatalogTypeId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("CustomizeColors")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("CustomizeDescription")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("CustomizeName")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("CustomizeSide")
                        .HasColumnType("varchar(100) CHARACTER SET utf8mb4")
                        .HasMaxLength(100);

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("BasketId");

                    b.ToTable("BasketItem");
                });

            modelBuilder.Entity("ApplicationCore.Entities.CatalogAttribute", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("CatalogItemId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(100) CHARACTER SET utf8mb4")
                        .HasMaxLength(100);

                    b.Property<int>("Stock")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CatalogItemId");

                    b.ToTable("CatalogAttribute");
                });

            modelBuilder.Entity("ApplicationCore.Entities.CatalogCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("CatalogItemId")
                        .HasColumnType("int");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CatalogItemId");

                    b.HasIndex("CategoryId");

                    b.ToTable("CatalogCategory");
                });

            modelBuilder.Entity("ApplicationCore.Entities.CatalogIllustration", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("varchar(25) CHARACTER SET utf8mb4")
                        .HasMaxLength(25);

                    b.Property<int>("IllustrationTypeId")
                        .HasColumnType("int");

                    b.Property<byte[]>("Image")
                        .HasColumnType("longblob");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(100) CHARACTER SET utf8mb4")
                        .HasMaxLength(100);

                    b.Property<string>("PictureUri")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.HasIndex("Code")
                        .IsUnique();

                    b.HasIndex("IllustrationTypeId");

                    b.ToTable("CatalogIllustration");
                });

            modelBuilder.Entity("ApplicationCore.Entities.CatalogItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<bool>("CanCustomize")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("CatalogIllustrationId")
                        .HasColumnType("int");

                    b.Property<int>("CatalogTypeId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<decimal?>("Discount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<bool>("IsFeatured")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsNew")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsUnavailable")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("MetaDescription")
                        .HasColumnType("varchar(161) CHARACTER SET utf8mb4")
                        .HasMaxLength(161);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(100) CHARACTER SET utf8mb4")
                        .HasMaxLength(100);

                    b.Property<string>("PictureUri")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<decimal?>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<bool>("ShowOnShop")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Sku")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4")
                        .HasMaxLength(255);

                    b.Property<string>("Slug")
                        .HasColumnType("varchar(100) CHARACTER SET utf8mb4")
                        .HasMaxLength(100);

                    b.Property<int>("Stock")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .HasColumnType("varchar(61) CHARACTER SET utf8mb4")
                        .HasMaxLength(61);

                    b.HasKey("Id");

                    b.HasIndex("CatalogIllustrationId");

                    b.HasIndex("CatalogTypeId");

                    b.HasIndex("Slug")
                        .IsUnique();

                    b.ToTable("Catalog");
                });

            modelBuilder.Entity("ApplicationCore.Entities.CatalogPicture", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("CatalogItemId")
                        .HasColumnType("int");

                    b.Property<bool>("IsActive")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsMain")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("Order")
                        .HasColumnType("int");

                    b.Property<string>("PictureHighUri")
                        .HasColumnType("varchar(1000) CHARACTER SET utf8mb4")
                        .HasMaxLength(1000);

                    b.Property<string>("PictureLowUri")
                        .HasColumnType("varchar(1000) CHARACTER SET utf8mb4")
                        .HasMaxLength(1000);

                    b.Property<string>("PictureUri")
                        .IsRequired()
                        .HasColumnType("varchar(1000) CHARACTER SET utf8mb4")
                        .HasMaxLength(1000);

                    b.HasKey("Id");

                    b.HasIndex("CatalogItemId");

                    b.ToTable("CatalogPicture");
                });

            modelBuilder.Entity("ApplicationCore.Entities.CatalogReference", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("CatalogItemId")
                        .HasColumnType("int");

                    b.Property<string>("LabelDescription")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(20) CHARACTER SET utf8mb4")
                        .HasMaxLength(20)
                        .HasDefaultValue("Tamanho");

                    b.Property<int>("ReferenceCatalogItemId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CatalogItemId");

                    b.HasIndex("ReferenceCatalogItemId");

                    b.ToTable("CatalogReference");
                });

            modelBuilder.Entity("ApplicationCore.Entities.CatalogType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<decimal?>("AdditionalTextPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("varchar(25) CHARACTER SET utf8mb4")
                        .HasMaxLength(25);

                    b.Property<int>("DeliveryTimeMax")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(3);

                    b.Property<int>("DeliveryTimeMin")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(2);

                    b.Property<int>("DeliveryTimeUnit")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<string>("Description")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("H1Text")
                        .HasColumnType("varchar(50) CHARACTER SET utf8mb4")
                        .HasMaxLength(50);

                    b.Property<string>("MetaDescription")
                        .HasColumnType("varchar(161) CHARACTER SET utf8mb4")
                        .HasMaxLength(161);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(100) CHARACTER SET utf8mb4")
                        .HasMaxLength(100);

                    b.Property<string>("PictureUri")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4")
                        .HasMaxLength(255);

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Question")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4")
                        .HasMaxLength(255);

                    b.Property<string>("Slug")
                        .HasColumnType("varchar(100) CHARACTER SET utf8mb4")
                        .HasMaxLength(100);

                    b.Property<string>("Title")
                        .HasColumnType("varchar(61) CHARACTER SET utf8mb4")
                        .HasMaxLength(61);

                    b.Property<int?>("Weight")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Code")
                        .IsUnique();

                    b.HasIndex("Slug")
                        .IsUnique();

                    b.ToTable("CatalogType");
                });

            modelBuilder.Entity("ApplicationCore.Entities.CatalogTypeCategory", b =>
                {
                    b.Property<int>("CatalogTypeId")
                        .HasColumnType("int");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.HasKey("CatalogTypeId", "CategoryId");

                    b.HasIndex("CategoryId");

                    b.ToTable("CatalogTypeCategory");
                });

            modelBuilder.Entity("ApplicationCore.Entities.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("H1Text")
                        .HasColumnType("varchar(50) CHARACTER SET utf8mb4")
                        .HasMaxLength(50);

                    b.Property<string>("MetaDescription")
                        .HasColumnType("varchar(161) CHARACTER SET utf8mb4")
                        .HasMaxLength(161);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(100) CHARACTER SET utf8mb4")
                        .HasMaxLength(100);

                    b.Property<int>("Order")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(1);

                    b.Property<int?>("ParentId")
                        .HasColumnType("int");

                    b.Property<string>("Slug")
                        .HasColumnType("varchar(100) CHARACTER SET utf8mb4")
                        .HasMaxLength(100);

                    b.Property<string>("Title")
                        .HasColumnType("varchar(61) CHARACTER SET utf8mb4")
                        .HasMaxLength(61);

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("ParentId");

                    b.HasIndex("Slug")
                        .IsUnique();

                    b.ToTable("Category");
                });

            modelBuilder.Entity("ApplicationCore.Entities.CustomizeOrder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("AttachFileName")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("BuyerContact")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("BuyerId")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("BuyerName")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Colors")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTimeOffset>("OrderDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("OrderState")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<string>("Text")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("CustomizeOrder");
                });

            modelBuilder.Entity("ApplicationCore.Entities.FileDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("CatalogTypeId")
                        .HasColumnType("int");

                    b.Property<string>("Extension")
                        .HasColumnType("varchar(10) CHARACTER SET utf8mb4")
                        .HasMaxLength(10);

                    b.Property<string>("FileName")
                        .HasColumnType("varchar(100) CHARACTER SET utf8mb4")
                        .HasMaxLength(100);

                    b.Property<bool?>("IsActive")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Location")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4")
                        .HasMaxLength(255);

                    b.Property<int?>("Order")
                        .HasColumnType("int");

                    b.Property<string>("PictureUri")
                        .IsRequired()
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.HasIndex("CatalogTypeId");

                    b.ToTable("FileDetail");
                });

            modelBuilder.Entity("ApplicationCore.Entities.IllustrationType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("varchar(25) CHARACTER SET utf8mb4")
                        .HasMaxLength(25);

                    b.Property<string>("Name")
                        .HasColumnType("varchar(100) CHARACTER SET utf8mb4")
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.HasIndex("Code")
                        .IsUnique();

                    b.ToTable("IllustrationType");
                });

            modelBuilder.Entity("ApplicationCore.Entities.OrderAggregate.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("BuyerId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Observations")
                        .HasColumnType("longtext CHARACTER SET utf8mb4")
                        .HasMaxLength(5000);

                    b.Property<DateTimeOffset>("OrderDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("OrderState")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("varchar(100) CHARACTER SET utf8mb4")
                        .HasMaxLength(100);

                    b.Property<long?>("SalesInvoiceId")
                        .HasColumnType("bigint");

                    b.Property<string>("SalesInvoiceNumber")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4")
                        .HasMaxLength(255);

                    b.Property<long?>("SalesPaymentId")
                        .HasColumnType("bigint");

                    b.Property<decimal>("ShippingCost")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int?>("TaxNumber")
                        .HasColumnType("int");

                    b.Property<bool>("UseBillingSameAsShipping")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("Id");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("ApplicationCore.Entities.OrderAggregate.OrderItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("CatalogAttribute1")
                        .HasColumnType("int");

                    b.Property<int?>("CatalogAttribute2")
                        .HasColumnType("int");

                    b.Property<int?>("CatalogAttribute3")
                        .HasColumnType("int");

                    b.Property<string>("CustomizeName")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("CustomizeSide")
                        .HasColumnType("varchar(100) CHARACTER SET utf8mb4")
                        .HasMaxLength(100);

                    b.Property<int?>("OrderId")
                        .HasColumnType("int");

                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("Units")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.ToTable("OrderItems");
                });

            modelBuilder.Entity("ApplicationCore.Entities.ShippingPriceWeight", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("Country")
                        .HasColumnType("int");

                    b.Property<int?>("MaxWeight")
                        .HasColumnType("int");

                    b.Property<int>("MinWeight")
                        .HasColumnType("int");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(65,30)");

                    b.HasKey("Id");

                    b.ToTable("ShippingPriceWeights");
                });

            modelBuilder.Entity("ApplicationCore.Entities.ShopConfig", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<bool>("IsActive")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(100) CHARACTER SET utf8mb4")
                        .HasMaxLength(100);

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<string>("Value")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.ToTable("ShopConfig");
                });

            modelBuilder.Entity("ApplicationCore.Entities.ShopConfigDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ContentText")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("HeadingText")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("IsActive")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("LinkButtonText")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("LinkButtonUri")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("PictureMobileUri")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4")
                        .HasMaxLength(255);

                    b.Property<string>("PictureUri")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4")
                        .HasMaxLength(255);

                    b.Property<string>("PictureWebpUri")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4")
                        .HasMaxLength(255);

                    b.Property<int>("ShopConfigId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ShopConfigId");

                    b.ToTable("ShopConfigDetail");
                });

            modelBuilder.Entity("ApplicationCore.Entities.BasketAggregate.BasketItem", b =>
                {
                    b.HasOne("ApplicationCore.Entities.BasketAggregate.Basket", "Basket")
                        .WithMany("Items")
                        .HasForeignKey("BasketId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ApplicationCore.Entities.CatalogAttribute", b =>
                {
                    b.HasOne("ApplicationCore.Entities.CatalogItem", "CatalogItem")
                        .WithMany("CatalogAttributes")
                        .HasForeignKey("CatalogItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ApplicationCore.Entities.CatalogCategory", b =>
                {
                    b.HasOne("ApplicationCore.Entities.CatalogItem", "CatalogItem")
                        .WithMany("CatalogCategories")
                        .HasForeignKey("CatalogItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ApplicationCore.Entities.Category", "Category")
                        .WithMany("CatalogCategories")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ApplicationCore.Entities.CatalogIllustration", b =>
                {
                    b.HasOne("ApplicationCore.Entities.IllustrationType", "IllustrationType")
                        .WithMany()
                        .HasForeignKey("IllustrationTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ApplicationCore.Entities.CatalogItem", b =>
                {
                    b.HasOne("ApplicationCore.Entities.CatalogIllustration", "CatalogIllustration")
                        .WithMany()
                        .HasForeignKey("CatalogIllustrationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ApplicationCore.Entities.CatalogType", "CatalogType")
                        .WithMany("CatalogItems")
                        .HasForeignKey("CatalogTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ApplicationCore.Entities.CatalogPicture", b =>
                {
                    b.HasOne("ApplicationCore.Entities.CatalogItem", "CatalogItem")
                        .WithMany("CatalogPictures")
                        .HasForeignKey("CatalogItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ApplicationCore.Entities.CatalogReference", b =>
                {
                    b.HasOne("ApplicationCore.Entities.CatalogItem", "CatalogItem")
                        .WithMany("CatalogReferences")
                        .HasForeignKey("CatalogItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ApplicationCore.Entities.CatalogItem", "ReferenceCatalogItem")
                        .WithMany()
                        .HasForeignKey("ReferenceCatalogItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ApplicationCore.Entities.CatalogTypeCategory", b =>
                {
                    b.HasOne("ApplicationCore.Entities.CatalogType", "CatalogType")
                        .WithMany("Categories")
                        .HasForeignKey("CatalogTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ApplicationCore.Entities.Category", "Category")
                        .WithMany("CatalogTypes")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ApplicationCore.Entities.Category", b =>
                {
                    b.HasOne("ApplicationCore.Entities.Category", "Parent")
                        .WithMany()
                        .HasForeignKey("ParentId");
                });

            modelBuilder.Entity("ApplicationCore.Entities.CustomizeOrder", b =>
                {
                    b.OwnsOne("ApplicationCore.Entities.OrderAggregate.CatalogItemOrdered", "ItemOrdered", b1 =>
                        {
                            b1.Property<int>("CustomizeOrderId")
                                .HasColumnType("int");

                            b1.Property<int>("CatalogItemId")
                                .HasColumnType("int");

                            b1.Property<string>("PictureUri")
                                .HasColumnType("longtext CHARACTER SET utf8mb4");

                            b1.Property<string>("ProductName")
                                .HasColumnType("longtext CHARACTER SET utf8mb4");

                            b1.HasKey("CustomizeOrderId");

                            b1.ToTable("CustomizeOrder");

                            b1.WithOwner()
                                .HasForeignKey("CustomizeOrderId");
                        });
                });

            modelBuilder.Entity("ApplicationCore.Entities.FileDetail", b =>
                {
                    b.HasOne("ApplicationCore.Entities.CatalogType", "CatalogType")
                        .WithMany("PictureTextHelpers")
                        .HasForeignKey("CatalogTypeId");
                });

            modelBuilder.Entity("ApplicationCore.Entities.OrderAggregate.Order", b =>
                {
                    b.OwnsOne("ApplicationCore.Entities.OrderAggregate.Address", "BillingToAddress", b1 =>
                        {
                            b1.Property<int>("OrderId")
                                .HasColumnType("int");

                            b1.Property<string>("City")
                                .HasColumnType("longtext CHARACTER SET utf8mb4");

                            b1.Property<string>("Country")
                                .HasColumnType("longtext CHARACTER SET utf8mb4");

                            b1.Property<string>("Name")
                                .HasColumnType("longtext CHARACTER SET utf8mb4");

                            b1.Property<string>("PostalCode")
                                .HasColumnType("longtext CHARACTER SET utf8mb4");

                            b1.Property<string>("Street")
                                .HasColumnType("longtext CHARACTER SET utf8mb4");

                            b1.HasKey("OrderId");

                            b1.ToTable("Orders");

                            b1.WithOwner()
                                .HasForeignKey("OrderId");
                        });

                    b.OwnsOne("ApplicationCore.Entities.OrderAggregate.Address", "ShipToAddress", b1 =>
                        {
                            b1.Property<int>("OrderId")
                                .HasColumnType("int");

                            b1.Property<string>("City")
                                .HasColumnType("longtext CHARACTER SET utf8mb4");

                            b1.Property<string>("Country")
                                .HasColumnType("longtext CHARACTER SET utf8mb4");

                            b1.Property<string>("Name")
                                .HasColumnType("longtext CHARACTER SET utf8mb4");

                            b1.Property<string>("PostalCode")
                                .HasColumnType("longtext CHARACTER SET utf8mb4");

                            b1.Property<string>("Street")
                                .HasColumnType("longtext CHARACTER SET utf8mb4");

                            b1.HasKey("OrderId");

                            b1.ToTable("Orders");

                            b1.WithOwner()
                                .HasForeignKey("OrderId");
                        });
                });

            modelBuilder.Entity("ApplicationCore.Entities.OrderAggregate.OrderItem", b =>
                {
                    b.HasOne("ApplicationCore.Entities.OrderAggregate.Order", null)
                        .WithMany("OrderItems")
                        .HasForeignKey("OrderId");

                    b.OwnsOne("ApplicationCore.Entities.OrderAggregate.CatalogItemOrdered", "ItemOrdered", b1 =>
                        {
                            b1.Property<int>("OrderItemId")
                                .HasColumnType("int");

                            b1.Property<int>("CatalogItemId")
                                .HasColumnType("int");

                            b1.Property<string>("PictureUri")
                                .HasColumnType("longtext CHARACTER SET utf8mb4");

                            b1.Property<string>("ProductName")
                                .HasColumnType("longtext CHARACTER SET utf8mb4");

                            b1.HasKey("OrderItemId");

                            b1.ToTable("OrderItems");

                            b1.WithOwner()
                                .HasForeignKey("OrderItemId");
                        });

                    b.OwnsOne("ApplicationCore.Entities.OrderAggregate.CustomizeItemOrdered", "CustomizeItem", b1 =>
                        {
                            b1.Property<int>("OrderItemId")
                                .HasColumnType("int");

                            b1.Property<int?>("CatalogTypeId")
                                .HasColumnType("int");

                            b1.Property<string>("Colors")
                                .HasColumnType("longtext CHARACTER SET utf8mb4");

                            b1.Property<string>("Description")
                                .HasColumnType("longtext CHARACTER SET utf8mb4");

                            b1.Property<string>("Name")
                                .HasColumnType("longtext CHARACTER SET utf8mb4");

                            b1.Property<string>("PictureUri")
                                .HasColumnType("longtext CHARACTER SET utf8mb4");

                            b1.Property<string>("ProductName")
                                .HasColumnType("longtext CHARACTER SET utf8mb4");

                            b1.HasKey("OrderItemId");

                            b1.ToTable("OrderItems");

                            b1.WithOwner()
                                .HasForeignKey("OrderItemId");
                        });
                });

            modelBuilder.Entity("ApplicationCore.Entities.ShopConfigDetail", b =>
                {
                    b.HasOne("ApplicationCore.Entities.ShopConfig", "ShopConfig")
                        .WithMany("Details")
                        .HasForeignKey("ShopConfigId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}