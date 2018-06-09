using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Infrastructure.Data.Migrations
{
    public partial class BasketAddress2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.RenameColumn(
                name: "ShipToAddress_UseBillingSameAsShipping",
                table: "Orders",
                newName: "UseBillingSameAsShipping");

            migrationBuilder.AlterColumn<bool>(
                name: "UseBillingSameAsShipping",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool));

            migrationBuilder.AddColumn<string>(
                name: "BillingToAddress_City",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingToAddress_Country",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingToAddress_PostalCode",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingToAddress_Street",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillingToAddress_City",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "BillingToAddress_Country",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "BillingToAddress_PostalCode",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "BillingToAddress_Street",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "UseBillingSameAsShipping",
                table: "Orders",
                newName: "ShipToAddress_UseBillingSameAsShipping");

            migrationBuilder.AlterColumn<bool>(
                name: "ShipToAddress_UseBillingSameAsShipping",
                table: "Orders",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "ShipToAddress_BillingCity",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShipToAddress_BillingCountry",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShipToAddress_BillingPostalCode",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShipToAddress_BillingStreet",
                table: "Orders",
                nullable: true);
        }
    }
}
