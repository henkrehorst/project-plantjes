using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace project_c.Migrations
{
    public partial class AddPlantData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Plants",
                columns: table => new
                {
                    PlantId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Length = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plants", x => x.PlantId);
                });

            migrationBuilder.CreateTable(
                name: "PlantPictures",
                columns: table => new
                {
                    PlantPicturesId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Source = table.Column<string>(nullable: true),
                    Position = table.Column<int>(nullable: false),
                    PlantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantPictures", x => x.PlantPicturesId);
                    table.ForeignKey(
                        name: "FK_PlantPictures_Plants_PlantId",
                        column: x => x.PlantId,
                        principalTable: "Plants",
                        principalColumn: "PlantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlantPictures_PlantId",
                table: "PlantPictures",
                column: "PlantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlantPictures");

            migrationBuilder.DropTable(
                name: "Plants");
        }
    }
}
