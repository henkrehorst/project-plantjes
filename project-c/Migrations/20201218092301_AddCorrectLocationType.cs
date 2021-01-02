using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

namespace project_c.Migrations
{
    public partial class AddCorrectLocationType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Point>(
                name: "Location",
                table: "User",
                type: "geography",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "User");
        }
    }
}
