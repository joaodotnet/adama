using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Infrastructure.Data.Migrations
{
    public partial class IllustrationType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Illustration");

            migrationBuilder.AddColumn<int>(
                name: "IllustrationTypeId",
                table: "Illustration",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "IllustrationType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IllustrationType", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Illustration_IllustrationTypeId",
                table: "Illustration",
                column: "IllustrationTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Illustration_IllustrationType_IllustrationTypeId",
                table: "Illustration",
                column: "IllustrationTypeId",
                principalTable: "IllustrationType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Illustration_IllustrationType_IllustrationTypeId",
                table: "Illustration");

            migrationBuilder.DropTable(
                name: "IllustrationType");

            migrationBuilder.DropIndex(
                name: "IX_Illustration_IllustrationTypeId",
                table: "Illustration");

            migrationBuilder.DropColumn(
                name: "IllustrationTypeId",
                table: "Illustration");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Illustration",
                nullable: false,
                defaultValue: 0);
        }
    }
}
