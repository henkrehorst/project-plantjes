using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace project_c.Migrations
{
    public partial class AddChatAndMessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChatDataInt",
                table: "User",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ChatDatasets",
                columns: table => new
                {
                    ChatDataInt = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatDatasets", x => x.ChatDataInt);
                });

            migrationBuilder.CreateTable(
                name: "Chats",
                columns: table => new
                {
                    ChatId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Created = table.Column<DateTime>(nullable: false),
                    ChatDataInt = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.ChatId);
                    table.ForeignKey(
                        name: "FK_Chats_ChatDatasets_ChatDataInt",
                        column: x => x.ChatDataInt,
                        principalTable: "ChatDatasets",
                        principalColumn: "ChatDataInt",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    MessageId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ChatId = table.Column<int>(nullable: false),
                    MessageContent = table.Column<string>(maxLength: 255, nullable: true),
                    ChatDataInt = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.MessageId);
                    table.ForeignKey(
                        name: "FK_Messages_ChatDatasets_ChatDataInt",
                        column: x => x.ChatDataInt,
                        principalTable: "ChatDatasets",
                        principalColumn: "ChatDataInt",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "ChatId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_User_ChatDataInt",
                table: "User",
                column: "ChatDataInt");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_ChatDataInt",
                table: "Chats",
                column: "ChatDataInt");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatDataInt",
                table: "Messages",
                column: "ChatDataInt");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatId",
                table: "Messages",
                column: "ChatId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_ChatDatasets_ChatDataInt",
                table: "User",
                column: "ChatDataInt",
                principalTable: "ChatDatasets",
                principalColumn: "ChatDataInt",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_ChatDatasets_ChatDataInt",
                table: "User");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Chats");

            migrationBuilder.DropTable(
                name: "ChatDatasets");

            migrationBuilder.DropIndex(
                name: "IX_User_ChatDataInt",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ChatDataInt",
                table: "User");
        }
    }
}
