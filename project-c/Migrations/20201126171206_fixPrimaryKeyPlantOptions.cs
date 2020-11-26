using Microsoft.EntityFrameworkCore.Migrations;

namespace project_c.Migrations
{
    public partial class fixPrimaryKeyPlantOptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PlantOptions",
                table: "PlantOptions");

            migrationBuilder.DropIndex(
                name: "IX_PlantOptions_FilterId",
                table: "PlantOptions");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlantOptions",
                table: "PlantOptions",
                columns: new[] { "FilterId", "PlantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PlantOptions_OptionId",
                table: "PlantOptions",
                column: "OptionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PlantOptions",
                table: "PlantOptions");

            migrationBuilder.DropIndex(
                name: "IX_PlantOptions_OptionId",
                table: "PlantOptions");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlantOptions",
                table: "PlantOptions",
                columns: new[] { "OptionId", "FilterId", "PlantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PlantOptions_FilterId",
                table: "PlantOptions",
                column: "FilterId");
        }
    }
}
