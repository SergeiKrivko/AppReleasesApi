using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppReleases.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class releaseassetsmanytomany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_Releases_ReleaseId",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_ReleaseId",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "ReleaseId",
                table: "Assets");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Assets",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ReleaseAssetEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReleaseId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssetId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReleaseAssetEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReleaseAssetEntity_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "AssetId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReleaseAssetEntity_Releases_ReleaseId",
                        column: x => x.ReleaseId,
                        principalTable: "Releases",
                        principalColumn: "ReleaseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReleaseAssetEntity_AssetId",
                table: "ReleaseAssetEntity",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_ReleaseAssetEntity_ReleaseId",
                table: "ReleaseAssetEntity",
                column: "ReleaseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReleaseAssetEntity");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Assets");

            migrationBuilder.AddColumn<Guid>(
                name: "ReleaseId",
                table: "Assets",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Assets_ReleaseId",
                table: "Assets",
                column: "ReleaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_Releases_ReleaseId",
                table: "Assets",
                column: "ReleaseId",
                principalTable: "Releases",
                principalColumn: "ReleaseId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
