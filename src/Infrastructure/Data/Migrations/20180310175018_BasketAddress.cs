using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Infrastructure.Data.Migrations
{
    public partial class BasketAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShipToAddress_BillingCity",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShipToAddress_BillingCountry",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShipToAddress_BillingPostalCode",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShipToAddress_BillingStreet",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ShipToAddress_UseBillingSameAsShipping",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShipToAddress_BillingCity",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShipToAddress_BillingCountry",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShipToAddress_BillingPostalCode",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShipToAddress_BillingStreet",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShipToAddress_UseBillingSameAsShipping",
                table: "Orders");
        }
    }
}
