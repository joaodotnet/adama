using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Grocery.Migrations
{
    public partial class BasketDates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "BasketItem",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "BasketItem",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Basket",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "Basket",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "BasketItem");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "BasketItem");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Basket");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "Basket");
        }
    }
}
