using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Infrastructure.Data.Migrations
{
    public partial class AddBackofficeDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Catalog_CatalogBrand_CatalogBrandId",
                table: "Catalog");

            migrationBuilder.DropTable(
                name: "CatalogBrand");

            migrationBuilder.DropIndex(
                name: "IX_Catalog_CatalogBrandId",
                table: "Catalog");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "CatalogType");

            migrationBuilder.DropColumn(
                name: "CatalogBrandId",
                table: "Catalog");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "CatalogType",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "CatalogType",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "CatalogType",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Catalog",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Catalog",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CatalogIllustrationId",
                table: "Catalog",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CatalogAttribute",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CatalogItemId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18, 2)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false)
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
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IllustrationType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IllustrationType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CatalogIllustration",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    IllustrationTypeId = table.Column<int>(type: "int", nullable: false),
                    Image = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PictureUri = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_CatalogType_CategoryId",
                table: "CatalogType",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogType_Code",
                table: "CatalogType",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_CatalogIllustrationId",
                table: "Catalog",
                column: "CatalogIllustrationId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogAttribute_CatalogItemId",
                table: "CatalogAttribute",
                column: "CatalogItemId");

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
                name: "IX_Category_Name",
                table: "Category",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IllustrationType_Code",
                table: "IllustrationType",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Catalog_CatalogIllustration_CatalogIllustrationId",
                table: "Catalog",
                column: "CatalogIllustrationId",
                principalTable: "CatalogIllustration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CatalogType_Category_CategoryId",
                table: "CatalogType",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Catalog_CatalogIllustration_CatalogIllustrationId",
                table: "Catalog");

            migrationBuilder.DropForeignKey(
                name: "FK_CatalogType_Category_CategoryId",
                table: "CatalogType");

            migrationBuilder.DropTable(
                name: "CatalogAttribute");

            migrationBuilder.DropTable(
                name: "CatalogIllustration");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "IllustrationType");

            migrationBuilder.DropIndex(
                name: "IX_CatalogType_CategoryId",
                table: "CatalogType");

            migrationBuilder.DropIndex(
                name: "IX_CatalogType_Code",
                table: "CatalogType");

            migrationBuilder.DropIndex(
                name: "IX_Catalog_CatalogIllustrationId",
                table: "Catalog");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "CatalogType");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "CatalogType");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "CatalogType");

            migrationBuilder.DropColumn(
                name: "CatalogIllustrationId",
                table: "Catalog");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "CatalogType",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Catalog",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Catalog",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CatalogBrandId",
                table: "Catalog",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CatalogBrand",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Brand = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogBrand", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_CatalogBrandId",
                table: "Catalog",
                column: "CatalogBrandId");

            migrationBuilder.AddForeignKey(
                name: "FK_Catalog_CatalogBrand_CatalogBrandId",
                table: "Catalog",
                column: "CatalogBrandId",
                principalTable: "CatalogBrand",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
