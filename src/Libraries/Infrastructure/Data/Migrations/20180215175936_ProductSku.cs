using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Infrastructure.Data.Migrations
{
    public partial class ProductSku : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Sku",
                table: "CatalogAttribute",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sku",
                table: "Catalog",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CatalogAttribute_Sku",
                table: "CatalogAttribute",
                column: "Sku",
                unique: true,
                filter: "[Sku] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_Sku",
                table: "Catalog",
                column: "Sku",
                unique: true,
                filter: "[Sku] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CatalogAttribute_Sku",
                table: "CatalogAttribute");

            migrationBuilder.DropIndex(
                name: "IX_Catalog_Sku",
                table: "Catalog");

            migrationBuilder.DropColumn(
                name: "Sku",
                table: "CatalogAttribute");

            migrationBuilder.DropColumn(
                name: "Sku",
                table: "Catalog");
        }
    }
}
