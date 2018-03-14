using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Infrastructure.Data.Migrations
{
    public partial class AddDeliveryTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeliveryTimeMax",
                table: "CatalogType",
                type: "int",
                nullable: false,
                defaultValue: 3);

            migrationBuilder.AddColumn<int>(
                name: "DeliveryTimeMin",
                table: "CatalogType",
                type: "int",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryTimeUnit",
                table: "CatalogType",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "dias");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryTimeMax",
                table: "CatalogType");

            migrationBuilder.DropColumn(
                name: "DeliveryTimeMin",
                table: "CatalogType");

            migrationBuilder.DropColumn(
                name: "DeliveryTimeUnit",
                table: "CatalogType");
        }
    }
}
