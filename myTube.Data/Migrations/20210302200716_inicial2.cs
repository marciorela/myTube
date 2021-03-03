using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace myTube.Data.Migrations
{
    public partial class inicial2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Assistido",
                table: "Filmes");

            migrationBuilder.DropColumn(
                name: "AssistidoEm",
                table: "Filmes");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataStatus",
                table: "Filmes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Filmes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataStatus",
                table: "Filmes");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Filmes");

            migrationBuilder.AddColumn<bool>(
                name: "Assistido",
                table: "Filmes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "AssistidoEm",
                table: "Filmes",
                type: "datetime2",
                nullable: true);
        }
    }
}
