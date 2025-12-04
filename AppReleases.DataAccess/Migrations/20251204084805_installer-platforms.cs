using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppReleases.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class installerplatforms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string[]>(
                name: "Platforms",
                table: "InstallerBuilderUsages",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Platforms",
                table: "InstallerBuilderUsages");
        }
    }
}
