using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddinvoiceToOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "SalesInvoiceId",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SalesInvoiceNumber",
                table: "Orders",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SalesPaymentId",
                table: "Orders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SalesInvoiceId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "SalesInvoiceNumber",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "SalesPaymentId",
                table: "Orders");
        }
    }
}
