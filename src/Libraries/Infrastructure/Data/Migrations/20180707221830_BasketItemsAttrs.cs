using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class BasketItemsAttrs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BasketDetailItem");

            migrationBuilder.AddColumn<int>(
                name: "CatalogAttribute1",
                table: "BasketItem",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CatalogAttribute2",
                table: "BasketItem",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CatalogAttribute3",
                table: "BasketItem",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CatalogAttribute1",
                table: "BasketItem");

            migrationBuilder.DropColumn(
                name: "CatalogAttribute2",
                table: "BasketItem");

            migrationBuilder.DropColumn(
                name: "CatalogAttribute3",
                table: "BasketItem");

            migrationBuilder.CreateTable(
                name: "BasketDetailItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BasketItemId = table.Column<int>(nullable: false),
                    CatalogAttributeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasketDetailItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BasketDetailItem_BasketItem_BasketItemId",
                        column: x => x.BasketItemId,
                        principalTable: "BasketItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BasketDetailItem_CatalogAttribute_CatalogAttributeId",
                        column: x => x.CatalogAttributeId,
                        principalTable: "CatalogAttribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BasketDetailItem_BasketItemId",
                table: "BasketDetailItem",
                column: "BasketItemId");

            migrationBuilder.CreateIndex(
                name: "IX_BasketDetailItem_CatalogAttributeId",
                table: "BasketDetailItem",
                column: "CatalogAttributeId");
        }
    }
}
