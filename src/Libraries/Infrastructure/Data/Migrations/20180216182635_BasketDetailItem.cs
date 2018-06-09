using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Infrastructure.Data.Migrations
{
    public partial class BasketDetailItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BasketDetailItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BasketItemId = table.Column<int>(type: "int", nullable: false),
                    CatalogAttributeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasketDetailItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BasketDetailItem_BasketItem_BasketItemId",
                        column: x => x.BasketItemId,
                        principalTable: "BasketItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BasketDetailItem_CatalogAttribute_CatalogAttributeId",
                        column: x => x.CatalogAttributeId,
                        principalTable: "CatalogAttribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BasketDetailItem_BasketItemId",
                table: "BasketDetailItem",
                column: "BasketItemId");

            migrationBuilder.CreateIndex(
                name: "IX_BasketDetailItem_CatalogAttributeId",
                table: "BasketDetailItem",
                column: "CatalogAttributeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BasketDetailItem");
        }
    }
}
