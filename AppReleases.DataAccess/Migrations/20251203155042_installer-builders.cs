using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppReleases.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class installerbuilders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InstallerBuilderUsages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuilderKey = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Settings = table.Column<string>(type: "jsonb", maxLength: 1024, nullable: true),
                    InstallerLifetime = table.Column<TimeSpan>(type: "interval", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstallerBuilderUsages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InstallerBuilderUsages_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BuiltInstallers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReleaseId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuilderId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuiltInstallers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BuiltInstallers_InstallerBuilderUsages_BuilderId",
                        column: x => x.BuilderId,
                        principalTable: "InstallerBuilderUsages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BuiltInstallers_Releases_ReleaseId",
                        column: x => x.ReleaseId,
                        principalTable: "Releases",
                        principalColumn: "ReleaseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BuiltInstallers_BuilderId",
                table: "BuiltInstallers",
                column: "BuilderId");

            migrationBuilder.CreateIndex(
                name: "IX_BuiltInstallers_ReleaseId",
                table: "BuiltInstallers",
                column: "ReleaseId");

            migrationBuilder.CreateIndex(
                name: "IX_InstallerBuilderUsages_ApplicationId",
                table: "InstallerBuilderUsages",
                column: "ApplicationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BuiltInstallers");

            migrationBuilder.DropTable(
                name: "InstallerBuilderUsages");
        }
    }
}
