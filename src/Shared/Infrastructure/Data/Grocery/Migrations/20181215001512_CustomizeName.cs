using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Grocery.Migrations
{
    public partial class CustomizeName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomizeName",
                table: "BasketItem",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomizeName",
                table: "BasketItem");
        }
    }
}
