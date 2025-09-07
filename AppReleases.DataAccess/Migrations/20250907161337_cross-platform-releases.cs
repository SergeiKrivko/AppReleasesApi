using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppReleases.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class crossplatformreleases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReleaseAssetEntity_Assets_AssetId",
                table: "ReleaseAssetEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_ReleaseAssetEntity_Releases_ReleaseId",
                table: "ReleaseAssetEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReleaseAssetEntity",
                table: "ReleaseAssetEntity");

            migrationBuilder.RenameTable(
                name: "ReleaseAssetEntity",
                newName: "ReleaseAssets");

            migrationBuilder.RenameIndex(
                name: "IX_ReleaseAssetEntity_ReleaseId",
                table: "ReleaseAssets",
                newName: "IX_ReleaseAssets_ReleaseId");

            migrationBuilder.RenameIndex(
                name: "IX_ReleaseAssetEntity_AssetId",
                table: "ReleaseAssets",
                newName: "IX_ReleaseAssets_AssetId");

            migrationBuilder.AlterColumn<string>(
                name: "Platform",
                table: "Releases",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReleaseAssets",
                table: "ReleaseAssets",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReleaseAssets_Assets_AssetId",
                table: "ReleaseAssets",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "AssetId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReleaseAssets_Releases_ReleaseId",
                table: "ReleaseAssets",
                column: "ReleaseId",
                principalTable: "Releases",
                principalColumn: "ReleaseId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReleaseAssets_Assets_AssetId",
                table: "ReleaseAssets");

            migrationBuilder.DropForeignKey(
                name: "FK_ReleaseAssets_Releases_ReleaseId",
                table: "ReleaseAssets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReleaseAssets",
                table: "ReleaseAssets");

            migrationBuilder.RenameTable(
                name: "ReleaseAssets",
                newName: "ReleaseAssetEntity");

            migrationBuilder.RenameIndex(
                name: "IX_ReleaseAssets_ReleaseId",
                table: "ReleaseAssetEntity",
                newName: "IX_ReleaseAssetEntity_ReleaseId");

            migrationBuilder.RenameIndex(
                name: "IX_ReleaseAssets_AssetId",
                table: "ReleaseAssetEntity",
                newName: "IX_ReleaseAssetEntity_AssetId");

            migrationBuilder.AlterColumn<string>(
                name: "Platform",
                table: "Releases",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReleaseAssetEntity",
                table: "ReleaseAssetEntity",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReleaseAssetEntity_Assets_AssetId",
                table: "ReleaseAssetEntity",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "AssetId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReleaseAssetEntity_Releases_ReleaseId",
                table: "ReleaseAssetEntity",
                column: "ReleaseId",
                principalTable: "Releases",
                principalColumn: "ReleaseId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
