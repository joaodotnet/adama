using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Infrastructure.Data.Migrations
{
    public partial class CustomizeOrder2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachFileContentType",
                table: "CustomizeOrder");

            migrationBuilder.DropColumn(
                name: "AttachFileUri",
                table: "CustomizeOrder");

            migrationBuilder.AddColumn<string>(
                name: "AttachFileName",
                table: "CustomizeOrder",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachFileName",
                table: "CustomizeOrder");

            migrationBuilder.AddColumn<string>(
                name: "AttachFileContentType",
                table: "CustomizeOrder",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachFileUri",
                table: "CustomizeOrder",
                nullable: true);
        }
    }
}
