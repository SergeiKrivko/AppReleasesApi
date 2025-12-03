using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppReleases.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class renameinstallers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(name: "Installers", newName: "Bundles");
            migrationBuilder.RenameColumn(name: "InstallerId", table: "Bundles", newName: "BundleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(name: "Bundles", newName: "Installers");
            migrationBuilder.RenameColumn(name: "BundleId", table: "Installers", newName: "InstallerId");
        }
    }
}
