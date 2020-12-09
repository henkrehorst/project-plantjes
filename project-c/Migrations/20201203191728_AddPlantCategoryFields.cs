using Microsoft.EntityFrameworkCore.Migrations;

namespace project_c.Migrations
{
    public partial class AddPlantCategoryFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Aanbod",
                table: "Plants",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Licht",
                table: "Plants",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Soort",
                table: "Plants",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Water",
                table: "Plants",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Aanbod",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "Licht",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "Soort",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "Water",
                table: "Plants");
        }
    }
}
