using Microsoft.EntityFrameworkCore.Migrations;

namespace project_c.Migrations
{
    public partial class emailRecoveryCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailRecoveryCode",
                table: "User",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailRecoveryCode",
                table: "User");
        }
    }
}
