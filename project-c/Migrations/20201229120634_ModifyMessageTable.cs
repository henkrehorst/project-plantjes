using Microsoft.EntityFrameworkCore.Migrations;

namespace project_c.Migrations
{
    public partial class ModifyMessageTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Messages");

            migrationBuilder.AddColumn<string>(
                name: "ReceivedUserId",
                table: "Messages",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceivedUserId",
                table: "Messages");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Messages",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
