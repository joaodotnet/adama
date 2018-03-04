using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Infrastructure.Data.Migrations
{
    public partial class RemoveCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttributeCode",
                table: "OrderItemDetails");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "CatalogAttribute");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AttributeCode",
                table: "OrderItemDetails",
                maxLength: 25,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "CatalogAttribute",
                maxLength: 25,
                nullable: false,
                defaultValue: "");
        }
    }
}
