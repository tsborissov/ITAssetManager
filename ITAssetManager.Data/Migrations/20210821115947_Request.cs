using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ITAssetManager.Data.Migrations
{
    public partial class Request : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Requests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubmitterId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AssetModelId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SubmissionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewerId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Rationale = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CloseComment = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Requests_AspNetUsers_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Requests_AspNetUsers_SubmitterId",
                        column: x => x.SubmitterId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Requests_AssetModels_AssetModelId",
                        column: x => x.AssetModelId,
                        principalTable: "AssetModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Requests_AssetModelId",
                table: "Requests",
                column: "AssetModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_ReviewerId",
                table: "Requests",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_SubmitterId",
                table: "Requests",
                column: "SubmitterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Requests");
        }
    }
}
