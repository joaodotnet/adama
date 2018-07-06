using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AttributesNew : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "CatalogAttribute");

            migrationBuilder.DropColumn(
                name: "Sku",
                table: "CatalogAttribute");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "CatalogAttribute",
                newName: "AttributeId");

            migrationBuilder.CreateTable(
                name: "Attribute",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attribute", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CatalogPrice",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CatalogItemId = table.Column<int>(nullable: false),
                    Attribute1Id = table.Column<int>(nullable: true),
                    Attribute2Id = table.Column<int>(nullable: true),
                    Attribute3Id = table.Column<int>(nullable: true),
                    Price = table.Column<decimal>(nullable: false),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogPrice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatalogPrice_Attribute_Attribute1Id",
                        column: x => x.Attribute1Id,
                        principalTable: "Attribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CatalogPrice_Attribute_Attribute2Id",
                        column: x => x.Attribute2Id,
                        principalTable: "Attribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CatalogPrice_Attribute_Attribute3Id",
                        column: x => x.Attribute3Id,
                        principalTable: "Attribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CatalogPrice_Catalog_CatalogItemId",
                        column: x => x.CatalogItemId,
                        principalTable: "Catalog",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CatalogAttribute_AttributeId",
                table: "CatalogAttribute",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogPrice_Attribute1Id",
                table: "CatalogPrice",
                column: "Attribute1Id");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogPrice_Attribute2Id",
                table: "CatalogPrice",
                column: "Attribute2Id");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogPrice_Attribute3Id",
                table: "CatalogPrice",
                column: "Attribute3Id");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogPrice_CatalogItemId",
                table: "CatalogPrice",
                column: "CatalogItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_CatalogAttribute_Attribute_AttributeId",
                table: "CatalogAttribute",
                column: "AttributeId",
                principalTable: "Attribute",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CatalogAttribute_Attribute_AttributeId",
                table: "CatalogAttribute");

            migrationBuilder.DropTable(
                name: "CatalogPrice");

            migrationBuilder.DropTable(
                name: "Attribute");

            migrationBuilder.DropIndex(
                name: "IX_CatalogAttribute_AttributeId",
                table: "CatalogAttribute");

            migrationBuilder.RenameColumn(
                name: "AttributeId",
                table: "CatalogAttribute",
                newName: "Type");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "CatalogAttribute",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Sku",
                table: "CatalogAttribute",
                maxLength: 255,
                nullable: true);
        }
    }
}
