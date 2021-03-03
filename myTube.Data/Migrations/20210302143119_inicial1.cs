using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace myTube.Data.Migrations
{
    public partial class inicial1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UltimoVideo",
                table: "Canais",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UltimoVideo",
                table: "Canais");
        }
    }
}
