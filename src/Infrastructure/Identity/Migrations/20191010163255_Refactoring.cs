using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Identity.Migrations
{
    public partial class Refactoring : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillingAddressSameAsShipping",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<bool>(
                name: "BillingAddressSameAsShipping",
                table: "UserAddress",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactNumber",
                table: "UserAddress",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "UserAddress",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TaxNumber",
                table: "UserAddress",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Gender",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillingAddressSameAsShipping",
                table: "UserAddress");

            migrationBuilder.DropColumn(
                name: "ContactNumber",
                table: "UserAddress");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "UserAddress");

            migrationBuilder.DropColumn(
                name: "TaxNumber",
                table: "UserAddress");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<bool>(
                name: "BillingAddressSameAsShipping",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);
        }
    }
}
