using Microsoft.EntityFrameworkCore.Migrations;

namespace Portal.Data.Migrations
{
    public partial class UI : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_EditorId",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "PollAnswer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_EditorId",
                table: "Users",
                column: "EditorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_EditorId",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "PollAnswer",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.CreateIndex(
                name: "IX_Users_EditorId",
                table: "Users",
                column: "EditorId",
                unique: false,
                filter: "[EditorId] IS NOT NULL");
        }
    }
}
