using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Layer.Migrations
{
    /// <inheritdoc />
    public partial class ClubTeamLeagueRobot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GameDate",
                table: "games");

            migrationBuilder.RenameColumn(
                name: "IsBatchProcessing",
                table: "games",
                newName: "IsProcessing");

            migrationBuilder.AddColumn<int>(
                name: "CurrentCupRound",
                table: "games",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrentLeagueRound",
                table: "games",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Day",
                table: "games",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DaysBetweenGames",
                table: "games",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "GameVersion",
                table: "games",
                type: "text",
                nullable: false,
                defaultValue: "");

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

            migrationBuilder.AddColumn<int>(
                name: "NumberOfTeamsInLeague",
                table: "games",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "games",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "clubs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    PlanetName = table.Column<string>(type: "text", nullable: false),
                    FoundationYear = table.Column<int>(type: "integer", nullable: false),
                    PrimaryColor = table.Column<string>(type: "text", nullable: false),
                    SecondaryColor = table.Column<string>(type: "text", nullable: false),
                    TertiaryColor = table.Column<string>(type: "text", nullable: false),
                    UniformStyle = table.Column<string>(type: "text", nullable: false),
                    LogoReference = table.Column<string>(type: "text", nullable: false),
                    Balance = table.Column<int>(type: "integer", nullable: false),
                    LastIncome = table.Column<int>(type: "integer", nullable: false),
                    LastExpenses = table.Column<int>(type: "integer", nullable: false),
                    FanCount = table.Column<int>(type: "integer", nullable: false),
                    FanHappiness = table.Column<int>(type: "integer", nullable: false),
                    FanExpectations = table.Column<int>(type: "integer", nullable: false),
                    ClubRankingPoints = table.Column<int>(type: "integer", nullable: false),
                    SeasonGamesWon = table.Column<int>(type: "integer", nullable: false),
                    SeasonGamesLost = table.Column<int>(type: "integer", nullable: false),
                    SeasonGamesDrawn = table.Column<int>(type: "integer", nullable: false),
                    SeasonGoalsScored = table.Column<int>(type: "integer", nullable: false),
                    SeasonGoalsConceded = table.Column<int>(type: "integer", nullable: false),
                    WinningStreak = table.Column<int>(type: "integer", nullable: false),
                    LosingStreak = table.Column<int>(type: "integer", nullable: false),
                    AllTimeGamesWon = table.Column<int>(type: "integer", nullable: false),
                    AllTimeGamesLost = table.Column<int>(type: "integer", nullable: false),
                    AllTimeGamesDrawn = table.Column<int>(type: "integer", nullable: false),
                    AllTimeGoalsScored = table.Column<int>(type: "integer", nullable: false),
                    AllTimeGoalsConceded = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clubs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "players",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "teams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_teams", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "clubs");

            migrationBuilder.DropTable(
                name: "players");

            migrationBuilder.DropTable(
                name: "teams");

            migrationBuilder.DropColumn(
                name: "CurrentCupRound",
                table: "games");

            migrationBuilder.DropColumn(
                name: "CurrentLeagueRound",
                table: "games");

            migrationBuilder.DropColumn(
                name: "Day",
                table: "games");

            migrationBuilder.DropColumn(
                name: "DaysBetweenGames",
                table: "games");

            migrationBuilder.DropColumn(
                name: "GameVersion",
                table: "games");

            migrationBuilder.DropColumn(
                name: "MaxCupRounds",
                table: "games");

            migrationBuilder.DropColumn(
                name: "MaxLeagueRounds",
                table: "games");

            migrationBuilder.DropColumn(
                name: "NumberOfTeamsInLeague",
                table: "games");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "games");

            migrationBuilder.RenameColumn(
                name: "IsProcessing",
                table: "games",
                newName: "IsBatchProcessing");

            migrationBuilder.AddColumn<DateTime>(
                name: "GameDate",
                table: "games",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
