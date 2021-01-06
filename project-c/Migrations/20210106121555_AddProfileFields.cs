using Microsoft.EntityFrameworkCore.Migrations;

namespace project_c.Migrations
{
    public partial class AddProfileFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Bio",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileBanner",
                table: "User",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bio",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ProfileBanner",
                table: "User");
        }
    }
}
