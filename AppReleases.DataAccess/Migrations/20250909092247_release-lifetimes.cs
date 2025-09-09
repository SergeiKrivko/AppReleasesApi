using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppReleases.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class releaselifetimes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UseDefaultDuration",
                table: "Branches",
                newName: "UseDefaultReleaseLifetime");

            migrationBuilder.RenameColumn(
                name: "Duration",
                table: "Branches",
                newName: "ReleaseLifetime");

            migrationBuilder.RenameColumn(
                name: "DefaultDuration",
                table: "Applications",
                newName: "DefaultReleaseLifetime");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "LatestReleaseLifetime",
                table: "Branches",
                type: "interval",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "DefaultLatestReleaseLifetime",
                table: "Applications",
                type: "interval",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LatestReleaseLifetime",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "DefaultLatestReleaseLifetime",
                table: "Applications");

            migrationBuilder.RenameColumn(
                name: "UseDefaultReleaseLifetime",
                table: "Branches",
                newName: "UseDefaultDuration");

            migrationBuilder.RenameColumn(
                name: "ReleaseLifetime",
                table: "Branches",
                newName: "Duration");

            migrationBuilder.RenameColumn(
                name: "DefaultReleaseLifetime",
                table: "Applications",
                newName: "DefaultDuration");
        }
    }
}
