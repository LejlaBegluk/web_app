using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Portal.Data.Migrations
{
    public partial class Correspondent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CorrespondentArticles",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Content = table.Column<string>(nullable: true),
                    CreateOn = table.Column<DateTime>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorrespondentArticles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CorrespondentArticles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CorrespondentArticlePhotos",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Url = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    DateAdded = table.Column<DateTime>(nullable: false),
                    PublicId = table.Column<string>(nullable: true),
                    CorrespondentArticleId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorrespondentArticlePhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CorrespondentArticlePhotos_CorrespondentArticles_CorrespondentArticleId",
                        column: x => x.CorrespondentArticleId,
                        principalTable: "CorrespondentArticles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CorrespondentArticlePhotos_CorrespondentArticleId",
                table: "CorrespondentArticlePhotos",
                column: "CorrespondentArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_CorrespondentArticles_UserId",
                table: "CorrespondentArticles",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CorrespondentArticlePhotos");

            migrationBuilder.DropTable(
                name: "CorrespondentArticles");
        }
    }
}
