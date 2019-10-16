using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class CatalogPicturesV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PictureUri",
                table: "CatalogPicture",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<bool>(
                name: "IsMain",
                table: "CatalogPicture",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PictureHighUri",
                table: "CatalogPicture",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PictureLowUri",
                table: "CatalogPicture",
                maxLength: 1000,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMain",
                table: "CatalogPicture");

            migrationBuilder.DropColumn(
                name: "PictureHighUri",
                table: "CatalogPicture");

            migrationBuilder.DropColumn(
                name: "PictureLowUri",
                table: "CatalogPicture");

            migrationBuilder.AlterColumn<string>(
                name: "PictureUri",
                table: "CatalogPicture",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 1000);
        }
    }
}
