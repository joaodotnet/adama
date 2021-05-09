using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class FixDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "CatalogType");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "CatalogType",
                newName: "Name");

            migrationBuilder.AddColumn<string>(
               name: "Description",
               table: "CatalogType",
               maxLength: 100,
               nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
               name: "Name",
               table: "CatalogType",
               nullable: true,
               defaultValue: "");

            migrationBuilder.RenameColumn(
               name: "Name",
               table: "CatalogType",
               newName: "Description");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "CatalogType");

        }
    }
}
