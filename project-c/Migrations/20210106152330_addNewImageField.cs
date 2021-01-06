using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace project_c.Migrations
{
    public partial class addNewImageField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string[]>(
                name: "Images",
                table: "Plants",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Images",
                table: "Plants");
        }
    }
}
