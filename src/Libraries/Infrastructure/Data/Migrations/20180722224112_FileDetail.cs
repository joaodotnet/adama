using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class FileDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileDetail",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PictureUri = table.Column<string>(maxLength: 255, nullable: false),
                    Location = table.Column<string>(maxLength: 255, nullable: true),
                    FileName = table.Column<string>(maxLength: 100, nullable: true),
                    Extension = table.Column<string>(maxLength: 10, nullable: true),
                    IsActive = table.Column<bool>(nullable: true),
                    Order = table.Column<int>(nullable: true),
                    CatalogTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileDetail_CatalogType_CatalogTypeId",
                        column: x => x.CatalogTypeId,
                        principalTable: "CatalogType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileDetail_CatalogTypeId",
                table: "FileDetail",
                column: "CatalogTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileDetail");
        }
    }
}
