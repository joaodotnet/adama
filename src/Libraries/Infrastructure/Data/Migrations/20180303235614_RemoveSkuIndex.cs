using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Infrastructure.Data.Migrations
{
    public partial class RemoveSkuIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CatalogAttribute_Sku",
                table: "CatalogAttribute");

            migrationBuilder.DropIndex(
                name: "IX_Catalog_Sku",
                table: "Catalog");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
