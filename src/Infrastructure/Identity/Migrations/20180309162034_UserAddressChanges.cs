using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Infrastructure.Identity.Migrations
{
    public partial class UserAddressChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultAddress",
                table: "UserAddress");

            migrationBuilder.AddColumn<int>(
                name: "AddressType",
                table: "UserAddress",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "BillingAddressSameAsShipping",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NIF",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressType",
                table: "UserAddress");

            migrationBuilder.DropColumn(
                name: "BillingAddressSameAsShipping",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NIF",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<bool>(
                name: "DefaultAddress",
                table: "UserAddress",
                nullable: false,
                defaultValue: false);
        }
    }
}
