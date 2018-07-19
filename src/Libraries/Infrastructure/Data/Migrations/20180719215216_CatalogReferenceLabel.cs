using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class CatalogReferenceLabel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LabelDescription",
                table: "CatalogReference",
                maxLength: 20,
                nullable: false,
                defaultValue: "Tamanho");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LabelDescription",
                table: "CatalogReference");
        }
    }
}
