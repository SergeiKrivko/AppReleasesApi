using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppReleases.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class assetisexecutable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsExecutable",
                table: "Assets",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsExecutable",
                table: "Assets");
        }
    }
}
