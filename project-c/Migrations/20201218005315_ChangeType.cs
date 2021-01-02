using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

namespace project_c.Migrations
{
    public partial class ChangeType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "User");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<NpgsqlPoint>(
                name: "Location",
                table: "User",
                type: "point",
                nullable: false,
                defaultValue: new NpgsqlTypes.NpgsqlPoint(0.0, 0.0));
        }
    }
}
