using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class InitialMySql : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Baskets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BuyerId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Baskets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CatalogType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(maxLength: 25, nullable: false),
                    Description = table.Column<string>(maxLength: 100, nullable: false),
                    PictureUri = table.Column<string>(maxLength: 255, nullable: true),
                    DeliveryTimeMin = table.Column<int>(nullable: false, defaultValue: 2),
                    DeliveryTimeMax = table.Column<int>(nullable: false, defaultValue: 3),
                    DeliveryTimeUnit = table.Column<int>(nullable: false, defaultValue: 0),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AdditionalTextPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ShippingCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 3.35m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Order = table.Column<int>(nullable: false, defaultValue: 1),
                    Position = table.Column<string>(maxLength: 10, nullable: false, defaultValue: "left"),
                    ParentId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Category_Category_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomizeOrder",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BuyerId = table.Column<string>(nullable: false),
                    BuyerName = table.Column<string>(nullable: false),
                    BuyerContact = table.Column<string>(nullable: false),
                    OrderDate = table.Column<DateTimeOffset>(nullable: false),
                    OrderState = table.Column<int>(nullable: false, defaultValue: 0),
                    ItemOrdered_CatalogItemId = table.Column<int>(nullable: false),
                    ItemOrdered_ProductName = table.Column<string>(nullable: true),
                    ItemOrdered_PictureUri = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    AttachFileName = table.Column<string>(nullable: true),
                    Colors = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomizeOrder", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IllustrationType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(maxLength: 25, nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IllustrationType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BuyerId = table.Column<string>(nullable: true),
                    TaxNumber = table.Column<int>(nullable: true),
                    PhoneNumber = table.Column<string>(maxLength: 100, nullable: true),
                    OrderDate = table.Column<DateTimeOffset>(nullable: false),
                    ShipToAddress_Name = table.Column<string>(nullable: true),
                    ShipToAddress_Street = table.Column<string>(nullable: true),
                    ShipToAddress_City = table.Column<string>(nullable: true),
                    ShipToAddress_Country = table.Column<string>(nullable: true),
                    ShipToAddress_PostalCode = table.Column<string>(nullable: true),
                    BillingToAddress_Name = table.Column<string>(nullable: true),
                    BillingToAddress_Street = table.Column<string>(nullable: true),
                    BillingToAddress_City = table.Column<string>(nullable: true),
                    BillingToAddress_Country = table.Column<string>(nullable: true),
                    BillingToAddress_PostalCode = table.Column<string>(nullable: true),
                    ShippingCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UseBillingSameAsShipping = table.Column<bool>(nullable: false),
                    OrderState = table.Column<int>(nullable: false, defaultValue: 0),
                    SalesInvoiceId = table.Column<long>(nullable: true),
                    SalesInvoiceNumber = table.Column<string>(maxLength: 255, nullable: true),
                    SalesPaymentId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShopConfig",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    Value = table.Column<string>(maxLength: 255, nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BasketItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    CatalogItemId = table.Column<int>(nullable: false),
                    CatalogAttribute1 = table.Column<int>(nullable: true),
                    CatalogAttribute2 = table.Column<int>(nullable: true),
                    CatalogAttribute3 = table.Column<int>(nullable: true),
                    CustomizeName = table.Column<string>(maxLength: 100, nullable: true),
                    CustomizeSide = table.Column<string>(maxLength: 100, nullable: true),
                    BasketId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasketItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BasketItem_Baskets_BasketId",
                        column: x => x.BasketId,
                        principalTable: "Baskets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FileDetail",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PictureUri = table.Column<string>(maxLength: 255, nullable: false),
                    Location = table.Column<string>(maxLength: 255, nullable: true),
                    FileName = table.Column<string>(maxLength: 100, nullable: true),
                    Extension = table.Column<string>(maxLength: 10, nullable: true),
                    IsActive = table.Column<bool>(nullable: true),
                    Order = table.Column<int>(nullable: true),
                    CatalogTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileDetail_CatalogType_CatalogTypeId",
                        column: x => x.CatalogTypeId,
                        principalTable: "CatalogType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CatalogTypeCategory",
                columns: table => new
                {
                    CatalogTypeId = table.Column<int>(nullable: false),
                    CategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogTypeCategory", x => new { x.CatalogTypeId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_CatalogTypeCategory_CatalogType_CatalogTypeId",
                        column: x => x.CatalogTypeId,
                        principalTable: "CatalogType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CatalogTypeCategory_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CatalogIllustration",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(maxLength: 25, nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    PictureUri = table.Column<string>(maxLength: 255, nullable: true),
                    IllustrationTypeId = table.Column<int>(nullable: false),
                    Image = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogIllustration", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatalogIllustration_IllustrationType_IllustrationTypeId",
                        column: x => x.IllustrationTypeId,
                        principalTable: "IllustrationType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ItemOrdered_CatalogItemId = table.Column<int>(nullable: false),
                    ItemOrdered_ProductName = table.Column<string>(nullable: true),
                    ItemOrdered_PictureUri = table.Column<string>(nullable: true),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Units = table.Column<int>(nullable: false),
                    CatalogAttribute1 = table.Column<int>(nullable: true),
                    CatalogAttribute2 = table.Column<int>(nullable: true),
                    CatalogAttribute3 = table.Column<int>(nullable: true),
                    CustomizeName = table.Column<string>(maxLength: 100, nullable: true),
                    CustomizeSide = table.Column<string>(maxLength: 100, nullable: true),
                    OrderId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShopConfigDetail",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PictureUri = table.Column<string>(maxLength: 255, nullable: true),
                    HeadingText = table.Column<string>(nullable: true),
                    ContentText = table.Column<string>(nullable: true),
                    LinkButtonUri = table.Column<string>(nullable: true),
                    LinkButtonText = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    ShopConfigId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopConfigDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopConfigDetail_ShopConfig_ShopConfigId",
                        column: x => x.ShopConfigId,
                        principalTable: "ShopConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Catalog",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Sku = table.Column<string>(maxLength: 255, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PictureUri = table.Column<string>(nullable: true),
                    CatalogTypeId = table.Column<int>(nullable: false),
                    CatalogIllustrationId = table.Column<int>(nullable: false),
                    ShowOnShop = table.Column<bool>(nullable: false),
                    IsNew = table.Column<bool>(nullable: false),
                    IsFeatured = table.Column<bool>(nullable: false),
                    CanCustomize = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Catalog_CatalogIllustration_CatalogIllustrationId",
                        column: x => x.CatalogIllustrationId,
                        principalTable: "CatalogIllustration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Catalog_CatalogType_CatalogTypeId",
                        column: x => x.CatalogTypeId,
                        principalTable: "CatalogType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CatalogAttribute",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    CatalogItemId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogAttribute", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatalogAttribute_Catalog_CatalogItemId",
                        column: x => x.CatalogItemId,
                        principalTable: "Catalog",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CatalogCategory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CatalogItemId = table.Column<int>(nullable: false),
                    CategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatalogCategory_Catalog_CatalogItemId",
                        column: x => x.CatalogItemId,
                        principalTable: "Catalog",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CatalogCategory_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CatalogPicture",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PictureUri = table.Column<string>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    Order = table.Column<int>(nullable: false),
                    CatalogItemId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogPicture", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatalogPicture_Catalog_CatalogItemId",
                        column: x => x.CatalogItemId,
                        principalTable: "Catalog",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CatalogReference",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CatalogItemId = table.Column<int>(nullable: false),
                    ReferenceCatalogItemId = table.Column<int>(nullable: false),
                    LabelDescription = table.Column<string>(maxLength: 20, nullable: false, defaultValue: "Tamanho")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogReference", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatalogReference_Catalog_CatalogItemId",
                        column: x => x.CatalogItemId,
                        principalTable: "Catalog",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CatalogReference_Catalog_ReferenceCatalogItemId",
                        column: x => x.ReferenceCatalogItemId,
                        principalTable: "Catalog",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BasketItem_BasketId",
                table: "BasketItem",
                column: "BasketId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_CatalogIllustrationId",
                table: "Catalog",
                column: "CatalogIllustrationId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_CatalogTypeId",
                table: "Catalog",
                column: "CatalogTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogAttribute_CatalogItemId",
                table: "CatalogAttribute",
                column: "CatalogItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogCategory_CatalogItemId",
                table: "CatalogCategory",
                column: "CatalogItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogCategory_CategoryId",
                table: "CatalogCategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogIllustration_Code",
                table: "CatalogIllustration",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CatalogIllustration_IllustrationTypeId",
                table: "CatalogIllustration",
                column: "IllustrationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogPicture_CatalogItemId",
                table: "CatalogPicture",
                column: "CatalogItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogReference_CatalogItemId",
                table: "CatalogReference",
                column: "CatalogItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogReference_ReferenceCatalogItemId",
                table: "CatalogReference",
                column: "ReferenceCatalogItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogType_Code",
                table: "CatalogType",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CatalogTypeCategory_CategoryId",
                table: "CatalogTypeCategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Category_Name",
                table: "Category",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Category_ParentId",
                table: "Category",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_FileDetail_CatalogTypeId",
                table: "FileDetail",
                column: "CatalogTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_IllustrationType_Code",
                table: "IllustrationType",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopConfigDetail_ShopConfigId",
                table: "ShopConfigDetail",
                column: "ShopConfigId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BasketItem");

            migrationBuilder.DropTable(
                name: "CatalogAttribute");

            migrationBuilder.DropTable(
                name: "CatalogCategory");

            migrationBuilder.DropTable(
                name: "CatalogPicture");

            migrationBuilder.DropTable(
                name: "CatalogReference");

            migrationBuilder.DropTable(
                name: "CatalogTypeCategory");

            migrationBuilder.DropTable(
                name: "CustomizeOrder");

            migrationBuilder.DropTable(
                name: "FileDetail");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "ShopConfigDetail");

            migrationBuilder.DropTable(
                name: "Baskets");

            migrationBuilder.DropTable(
                name: "Catalog");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "ShopConfig");

            migrationBuilder.DropTable(
                name: "CatalogIllustration");

            migrationBuilder.DropTable(
                name: "CatalogType");

            migrationBuilder.DropTable(
                name: "IllustrationType");
        }
    }
}
