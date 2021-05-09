using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Identity.Migrations
{
    public partial class AuthConfigChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CallbackURL",
                table: "AuthConfig",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "AuthConfig",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientSecret",
                table: "AuthConfig",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SigningSecret",
                table: "AuthConfig",
                maxLength: 255,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CallbackURL",
                table: "AuthConfig");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "AuthConfig");

            migrationBuilder.DropColumn(
                name: "ClientSecret",
                table: "AuthConfig");

            migrationBuilder.DropColumn(
                name: "SigningSecret",
                table: "AuthConfig");
        }
    }
}
