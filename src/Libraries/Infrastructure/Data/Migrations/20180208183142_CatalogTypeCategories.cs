using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Infrastructure.Data.Migrations
{
    public partial class CatalogTypeCategories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CatalogType_Category_CategoryId",
                table: "CatalogType");

            migrationBuilder.DropIndex(
                name: "IX_CatalogType_CategoryId",
                table: "CatalogType");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "CatalogType");

            migrationBuilder.AddColumn<string>(
                name: "PictureUri",
                table: "CatalogType",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CatalogTypeCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CatalogTypeId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogTypeCategory", x => x.Id);
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

            migrationBuilder.CreateIndex(
                name: "IX_CatalogTypeCategory_CatalogTypeId",
                table: "CatalogTypeCategory",
                column: "CatalogTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogTypeCategory_CategoryId",
                table: "CatalogTypeCategory",
                column: "CategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CatalogTypeCategory");

            migrationBuilder.DropColumn(
                name: "PictureUri",
                table: "CatalogType");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "CatalogType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CatalogType_CategoryId",
                table: "CatalogType",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_CatalogType_Category_CategoryId",
                table: "CatalogType",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
