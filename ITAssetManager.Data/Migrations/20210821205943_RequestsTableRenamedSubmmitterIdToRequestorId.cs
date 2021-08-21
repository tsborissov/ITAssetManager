using Microsoft.EntityFrameworkCore.Migrations;

namespace ITAssetManager.Data.Migrations
{
    public partial class RequestsTableRenamedSubmmitterIdToRequestorId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_AspNetUsers_SubmitterId",
                table: "Requests");

            migrationBuilder.RenameColumn(
                name: "SubmitterId",
                table: "Requests",
                newName: "RequestorId");

            migrationBuilder.RenameIndex(
                name: "IX_Requests_SubmitterId",
                table: "Requests",
                newName: "IX_Requests_RequestorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_AspNetUsers_RequestorId",
                table: "Requests",
                column: "RequestorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_AspNetUsers_RequestorId",
                table: "Requests");

            migrationBuilder.RenameColumn(
                name: "RequestorId",
                table: "Requests",
                newName: "SubmitterId");

            migrationBuilder.RenameIndex(
                name: "IX_Requests_RequestorId",
                table: "Requests",
                newName: "IX_Requests_SubmitterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_AspNetUsers_SubmitterId",
                table: "Requests",
                column: "SubmitterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
