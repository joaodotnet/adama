using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class SEOAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Position",
                table: "Category");

            migrationBuilder.AddColumn<string>(
                name: "MetaDescription",
                table: "Category",
                maxLength: 161,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Category",
                maxLength: 61,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaDescription",
                table: "CatalogType",
                maxLength: 161,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "CatalogType",
                maxLength: 61,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaDescription",
                table: "Catalog",
                maxLength: 161,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Catalog",
                maxLength: 61,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MetaDescription",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "MetaDescription",
                table: "CatalogType");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "CatalogType");

            migrationBuilder.DropColumn(
                name: "MetaDescription",
                table: "Catalog");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Catalog");

            migrationBuilder.AddColumn<string>(
                name: "Position",
                table: "Category",
                maxLength: 10,
                nullable: false,
                defaultValue: "left");
        }
    }
}
