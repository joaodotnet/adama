using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Infrastructure.Data.Migrations
{
    public partial class CatalogTypeCategoryFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CatalogTypeCategory",
                table: "CatalogTypeCategory");

            migrationBuilder.DropIndex(
                name: "IX_CatalogTypeCategory_CatalogTypeId",
                table: "CatalogTypeCategory");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "CatalogTypeCategory");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CatalogTypeCategory",
                table: "CatalogTypeCategory",
                columns: new[] { "CatalogTypeId", "CategoryId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CatalogTypeCategory",
                table: "CatalogTypeCategory");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "CatalogTypeCategory",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CatalogTypeCategory",
                table: "CatalogTypeCategory",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogTypeCategory_CatalogTypeId",
                table: "CatalogTypeCategory",
                column: "CatalogTypeId");
        }
    }
}
