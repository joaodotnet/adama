using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class CatalogAttributesv5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CatalogAttribute_Attribute_AttributeId",
                table: "CatalogAttribute");

            migrationBuilder.DropForeignKey(
                name: "FK_CatalogAttribute_Catalog_ReferenceCatalogItemId",
                table: "CatalogAttribute");

            migrationBuilder.DropTable(
                name: "CatalogPrice");

            migrationBuilder.DropTable(
                name: "Attribute");

            migrationBuilder.DropIndex(
                name: "IX_CatalogAttribute_AttributeId",
                table: "CatalogAttribute");

            migrationBuilder.DropIndex(
                name: "IX_CatalogAttribute_ReferenceCatalogItemId",
                table: "CatalogAttribute");

            migrationBuilder.DropColumn(
                name: "AttributeId",
                table: "CatalogAttribute");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "CatalogAttribute");

            migrationBuilder.DropColumn(
                name: "ReferenceCatalogItemId",
                table: "CatalogAttribute");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "CatalogAttribute",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "CatalogAttribute",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CatalogReference",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CatalogItemId = table.Column<int>(nullable: false),
                    ReferenceCatalogItemId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogReference", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatalogReference_Catalog_CatalogItemId",
                        column: x => x.CatalogItemId,
                        principalTable: "Catalog",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CatalogReference_Catalog_ReferenceCatalogItemId",
                        column: x => x.ReferenceCatalogItemId,
                        principalTable: "Catalog",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CatalogReference_CatalogItemId",
                table: "CatalogReference",
                column: "CatalogItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogReference_ReferenceCatalogItemId",
                table: "CatalogReference",
                column: "ReferenceCatalogItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CatalogReference");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "CatalogAttribute");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "CatalogAttribute");

            migrationBuilder.AddColumn<int>(
                name: "AttributeId",
                table: "CatalogAttribute",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "CatalogAttribute",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReferenceCatalogItemId",
                table: "CatalogAttribute",
                nullable: true);

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
                    Active = table.Column<bool>(nullable: false),
                    Attribute1Id = table.Column<int>(nullable: true),
                    Attribute2Id = table.Column<int>(nullable: true),
                    Attribute3Id = table.Column<int>(nullable: true),
                    CatalogItemId = table.Column<int>(nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
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
                name: "IX_CatalogAttribute_ReferenceCatalogItemId",
                table: "CatalogAttribute",
                column: "ReferenceCatalogItemId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_CatalogAttribute_Catalog_ReferenceCatalogItemId",
                table: "CatalogAttribute",
                column: "ReferenceCatalogItemId",
                principalTable: "Catalog",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
