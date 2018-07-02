using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class UseBillingSameAsShippingTypeChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "UseBillingSameAsShipping",
                table: "Orders",
                nullable: false,
                oldClrType: typeof(bool),
                oldDefaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "UseBillingSameAsShipping",
                table: "Orders",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool));
        }
    }
}
