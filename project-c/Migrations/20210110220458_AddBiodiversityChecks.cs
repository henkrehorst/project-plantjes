using Microsoft.EntityFrameworkCore.Migrations;

namespace project_c.Migrations
{
    public partial class AddBiodiversityChecks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "checkBees",
                table: "Plants",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "checkOtherAnimals",
                table: "Plants",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "checkOtherPlants",
                table: "Plants",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "checkBees",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "checkOtherAnimals",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "checkOtherPlants",
                table: "Plants");
        }
    }
}
