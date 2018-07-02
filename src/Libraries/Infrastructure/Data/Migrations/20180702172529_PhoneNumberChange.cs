using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class PhoneNumberChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillingToAddress_PhoneNumber",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "ShipToAddress_PhoneNumber",
                table: "Orders",
                newName: "PhoneNumber");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Orders",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "Orders",
                newName: "ShipToAddress_PhoneNumber");

            migrationBuilder.AlterColumn<int>(
                name: "ShipToAddress_PhoneNumber",
                table: "Orders",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BillingToAddress_PhoneNumber",
                table: "Orders",
                nullable: true);
        }
    }
}
