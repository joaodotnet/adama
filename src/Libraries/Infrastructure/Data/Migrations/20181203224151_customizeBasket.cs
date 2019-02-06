using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class customizeBasket : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomizeItem_CatalogTypeId",
                table: "OrderItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomizeItem_Colors",
                table: "OrderItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomizeItem_Description",
                table: "OrderItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomizeItem_Name",
                table: "OrderItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomizeItem_PictureUri",
                table: "OrderItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomizeItem_ProductName",
                table: "OrderItems",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CatalogTypeId",
                table: "BasketItem",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomizeColors",
                table: "BasketItem",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomizeDescription",
                table: "BasketItem",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomizeItem_CatalogTypeId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "CustomizeItem_Colors",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "CustomizeItem_Description",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "CustomizeItem_Name",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "CustomizeItem_PictureUri",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "CustomizeItem_ProductName",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "CatalogTypeId",
                table: "BasketItem");

            migrationBuilder.DropColumn(
                name: "CustomizeColors",
                table: "BasketItem");

            migrationBuilder.DropColumn(
                name: "CustomizeDescription",
                table: "BasketItem");
        }
    }
}
