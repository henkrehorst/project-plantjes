using Microsoft.EntityFrameworkCore.Migrations;

namespace project_c.Migrations
{
    public partial class addPlantOptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlantOptions",
                columns: table => new
                {
                    OptionId = table.Column<int>(nullable: false),
                    FilterId = table.Column<int>(nullable: false),
                    PlantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantOptions", x => new { x.OptionId, x.FilterId, x.PlantId });
                    table.ForeignKey(
                        name: "FK_PlantOptions_Filters_FilterId",
                        column: x => x.FilterId,
                        principalTable: "Filters",
                        principalColumn: "FilterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlantOptions_Options_OptionId",
                        column: x => x.OptionId,
                        principalTable: "Options",
                        principalColumn: "OptionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlantOptions_Plants_PlantId",
                        column: x => x.PlantId,
                        principalTable: "Plants",
                        principalColumn: "PlantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlantOptions_FilterId",
                table: "PlantOptions",
                column: "FilterId");

            migrationBuilder.CreateIndex(
                name: "IX_PlantOptions_PlantId",
                table: "PlantOptions",
                column: "PlantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlantOptions");
        }
    }
}
