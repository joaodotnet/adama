using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class shippingweight : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShippingCost",
                table: "CatalogType");

            migrationBuilder.AddColumn<decimal>(
                name: "Weight",
                table: "CatalogType",
                type: "decimal(18,2)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Weight",
                table: "CatalogType");

            migrationBuilder.AddColumn<decimal>(
                name: "ShippingCost",
                table: "CatalogType",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 3.35m);
        }
    }
}
