using Microsoft.EntityFrameworkCore.Migrations;

namespace project_c.Migrations
{
    public partial class IdentityUserRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "IdentityRoles",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "IdentityUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserRoles", x => new { x.UserId, x.RoleId });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IdentityUserRoles");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "IdentityRoles");
        }
    }
}
