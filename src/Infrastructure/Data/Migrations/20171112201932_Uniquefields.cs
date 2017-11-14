using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Infrastructure.Data.Migrations
{
    public partial class Uniquefields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ProductType_Code",
                table: "ProductType",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IllustrationType_Code",
                table: "IllustrationType",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Illustration_Code",
                table: "Illustration",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Category_Name",
                table: "Category",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductType_Code",
                table: "ProductType");

            migrationBuilder.DropIndex(
                name: "IX_IllustrationType_Code",
                table: "IllustrationType");

            migrationBuilder.DropIndex(
                name: "IX_Illustration_Code",
                table: "Illustration");

            migrationBuilder.DropIndex(
                name: "IX_Category_Name",
                table: "Category");
        }
    }
}
