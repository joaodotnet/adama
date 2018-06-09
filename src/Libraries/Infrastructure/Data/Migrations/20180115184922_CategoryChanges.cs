using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Infrastructure.Data.Migrations
{
    public partial class CategoryChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Category",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "Position",
                table: "Category",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "left");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "Category");
        }
    }
}
