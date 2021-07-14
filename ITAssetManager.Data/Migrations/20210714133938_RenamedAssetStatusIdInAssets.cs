using Microsoft.EntityFrameworkCore.Migrations;

namespace ITAssetManager.Data.Migrations
{
    public partial class RenamedAssetStatusIdInAssets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetModels_Brands_BrandId",
                table: "AssetModels");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetModels_Categories_CategoryId",
                table: "AssetModels");

            migrationBuilder.DropForeignKey(
                name: "FK_Assets_AssetModels_AssetModelId",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_Assets_Statuses_AssetStatusId",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_Assets_Vendors_VendorId",
                table: "Assets");

            migrationBuilder.RenameColumn(
                name: "AssetStatusId",
                table: "Assets",
                newName: "StatusId");

            migrationBuilder.RenameIndex(
                name: "IX_Assets_AssetStatusId",
                table: "Assets",
                newName: "IX_Assets_StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetModels_Brands_BrandId",
                table: "AssetModels",
                column: "BrandId",
                principalTable: "Brands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetModels_Categories_CategoryId",
                table: "AssetModels",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_AssetModels_AssetModelId",
                table: "Assets",
                column: "AssetModelId",
                principalTable: "AssetModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_Statuses_StatusId",
                table: "Assets",
                column: "StatusId",
                principalTable: "Statuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_Vendors_VendorId",
                table: "Assets",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetModels_Brands_BrandId",
                table: "AssetModels");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetModels_Categories_CategoryId",
                table: "AssetModels");

            migrationBuilder.DropForeignKey(
                name: "FK_Assets_AssetModels_AssetModelId",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_Assets_Statuses_StatusId",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_Assets_Vendors_VendorId",
                table: "Assets");

            migrationBuilder.RenameColumn(
                name: "StatusId",
                table: "Assets",
                newName: "AssetStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_Assets_StatusId",
                table: "Assets",
                newName: "IX_Assets_AssetStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetModels_Brands_BrandId",
                table: "AssetModels",
                column: "BrandId",
                principalTable: "Brands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetModels_Categories_CategoryId",
                table: "AssetModels",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_AssetModels_AssetModelId",
                table: "Assets",
                column: "AssetModelId",
                principalTable: "AssetModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_Statuses_AssetStatusId",
                table: "Assets",
                column: "AssetStatusId",
                principalTable: "Statuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_Vendors_VendorId",
                table: "Assets",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
