using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Infrastructure.Data.Migrations
{
    public partial class OrderItemDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderItemDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AttributeCode = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    AttributeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AttributeType = table.Column<int>(type: "int", nullable: false),
                    OrderItemId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItemDetails_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "OrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemDetails_OrderItemId",
                table: "OrderItemDetails",
                column: "OrderItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItemDetails");
        }
    }
}
