using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Database.Layer.Migrations
{
    /// <inheritdoc />
    public partial class AddCalendar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DaysBetweenGames",
                table: "games");

            migrationBuilder.DropColumn(
                name: "MaxCupRounds",
                table: "games");

            migrationBuilder.DropColumn(
                name: "MaxLeagueRounds",
                table: "games");

            migrationBuilder.CreateTable(
                name: "calender",
                columns: table => new
                {
                    DayIndex = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DayType = table.Column<int>(type: "integer", nullable: false),
                    CompetitionId = table.Column<int>(type: "integer", nullable: true),
                    CompetitionRound = table.Column<int>(type: "integer", nullable: true),
                    ScriptToRun = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_calender", x => x.DayIndex);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "calender");

            migrationBuilder.AddColumn<int>(
                name: "DaysBetweenGames",
                table: "games",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxCupRounds",
                table: "games",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxLeagueRounds",
                table: "games",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
