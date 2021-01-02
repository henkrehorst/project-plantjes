using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

namespace project_c.Migrations
{
    public partial class ChangeLoctionType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Lat",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Lng",
                table: "User");

            migrationBuilder.AddColumn<NpgsqlPoint>(
                name: "Location",
                table: "User",
                nullable: false,
                defaultValue: new NpgsqlTypes.NpgsqlPoint(0.0, 0.0));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "User");

            migrationBuilder.AddColumn<double>(
                name: "Lat",
                table: "User",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Lng",
                table: "User",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
