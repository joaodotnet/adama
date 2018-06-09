using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Infrastructure.Data.Migrations
{
    public partial class ProductAttributes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReferenceCatalogItemId",
                table: "CatalogAttribute",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CatalogAttribute_ReferenceCatalogItemId",
                table: "CatalogAttribute",
                column: "ReferenceCatalogItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_CatalogAttribute_Catalog_ReferenceCatalogItemId",
                table: "CatalogAttribute",
                column: "ReferenceCatalogItemId",
                principalTable: "Catalog",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CatalogAttribute_Catalog_ReferenceCatalogItemId",
                table: "CatalogAttribute");

            migrationBuilder.DropIndex(
                name: "IX_CatalogAttribute_ReferenceCatalogItemId",
                table: "CatalogAttribute");

            migrationBuilder.DropColumn(
                name: "ReferenceCatalogItemId",
                table: "CatalogAttribute");
        }
    }
}
