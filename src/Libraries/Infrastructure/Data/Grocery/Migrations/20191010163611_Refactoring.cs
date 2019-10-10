using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Grocery.Migrations
{
    public partial class Refactoring : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "CatalogType");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "CatalogType",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
