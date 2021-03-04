using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace myTube.Data.Migrations
{
    public partial class inicial3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledStartTime",
                table: "Filmes",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScheduledStartTime",
                table: "Filmes");
        }
    }
}
