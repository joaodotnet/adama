using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Grocery.Migrations.SqlServer
{
    public partial class InitialSqlServer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Basket",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BuyerId = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Basket", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CatalogType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(maxLength: 25, nullable: false),
                    Description = table.Column<string>(maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
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
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Order = table.Column<int>(nullable: false, defaultValue: 1),
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
                name: "Country",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Code = table.Column<string>(maxLength: 10, nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Country", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BuyerId = table.Column<string>(nullable: true),
                    CustomerEmail = table.Column<string>(nullable: true),
                    TaxNumber = table.Column<int>(nullable: true),
                    OrderDate = table.Column<DateTimeOffset>(nullable: false),
                    BillingToAddress_Name = table.Column<string>(nullable: true),
                    BillingToAddress_Street = table.Column<string>(nullable: true),
                    BillingToAddress_City = table.Column<string>(nullable: true),
                    BillingToAddress_Country = table.Column<string>(nullable: true),
                    BillingToAddress_PostalCode = table.Column<string>(nullable: true),
                    OrderState = table.Column<int>(nullable: false, defaultValue: 0),
                    SalesInvoiceId = table.Column<long>(nullable: true),
                    SalesInvoiceNumber = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BasketItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    CatalogItemId = table.Column<int>(nullable: false),
                    CatalogAttribute1 = table.Column<int>(nullable: true),
                    CatalogAttribute2 = table.Column<int>(nullable: true),
                    CatalogAttribute3 = table.Column<int>(nullable: true),
                    CustomizeName = table.Column<string>(maxLength: 100, nullable: true),
                    CustomizeDescription = table.Column<string>(nullable: true),
                    CustomizeColors = table.Column<string>(nullable: true),
                    CatalogTypeId = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    BasketId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasketItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BasketItem_Basket_BasketId",
                        column: x => x.BasketId,
                        principalTable: "Basket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Catalog",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Sku = table.Column<string>(maxLength: 255, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PictureUri = table.Column<string>(nullable: true),
                    CatalogTypeId = table.Column<int>(nullable: false),
                    ShowOnShop = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Catalog_CatalogType_CatalogTypeId",
                        column: x => x.CatalogTypeId,
                        principalTable: "CatalogType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ItemOrdered_CatalogItemId = table.Column<int>(nullable: false),
                    ItemOrdered_ProductName = table.Column<string>(nullable: true),
                    ItemOrdered_PictureUri = table.Column<string>(nullable: true),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Units = table.Column<int>(nullable: false),
                    CatalogAttribute1 = table.Column<int>(nullable: true),
                    CatalogAttribute2 = table.Column<int>(nullable: true),
                    CatalogAttribute3 = table.Column<int>(nullable: true),
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
                name: "CatalogAttribute",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
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
                    CatalogItemId = table.Column<int>(nullable: false),
                    CategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogCategory", x => new { x.CatalogItemId, x.CategoryId });
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

            migrationBuilder.CreateIndex(
                name: "IX_BasketItem_BasketId",
                table: "BasketItem",
                column: "BasketId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_CatalogTypeId",
                table: "Catalog",
                column: "CatalogTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogAttribute_CatalogItemId",
                table: "CatalogAttribute",
                column: "CatalogItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogCategory_CategoryId",
                table: "CatalogCategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogType_Code",
                table: "CatalogType",
                column: "Code",
                unique: true);

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
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");
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
                name: "Country");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Basket");

            migrationBuilder.DropTable(
                name: "Catalog");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "CatalogType");
        }
    }
}
