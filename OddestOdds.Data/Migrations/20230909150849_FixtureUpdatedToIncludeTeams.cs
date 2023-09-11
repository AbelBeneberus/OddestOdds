using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OddestOdds.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixtureUpdatedToIncludeTeams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AwayTeam",
                table: "Fixtures",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HomeTeam",
                table: "Fixtures",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AwayTeam",
                table: "Fixtures");

            migrationBuilder.DropColumn(
                name: "HomeTeam",
                table: "Fixtures");
        }
    }
}
