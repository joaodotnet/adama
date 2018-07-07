using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class OrderItemAttrs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItemDetails");

            migrationBuilder.AddColumn<int>(
                name: "CatalogAttribute1",
                table: "OrderItems",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CatalogAttribute2",
                table: "OrderItems",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CatalogAttribute3",
                table: "OrderItems",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CatalogAttribute1",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "CatalogAttribute2",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "CatalogAttribute3",
                table: "OrderItems");

            migrationBuilder.CreateTable(
                name: "OrderItemDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AttributeName = table.Column<string>(maxLength: 100, nullable: false),
                    AttributeType = table.Column<int>(nullable: false),
                    OrderItemId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItemDetails_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "OrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemDetails_OrderItemId",
                table: "OrderItemDetails",
                column: "OrderItemId");
        }
    }
}
