using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class PicturesWebp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PictureMobileUri",
                table: "ShopConfigDetail",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PictureWebpUri",
                table: "ShopConfigDetail",
                maxLength: 255,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PictureMobileUri",
                table: "ShopConfigDetail");

            migrationBuilder.DropColumn(
                name: "PictureWebpUri",
                table: "ShopConfigDetail");
        }
    }
}
