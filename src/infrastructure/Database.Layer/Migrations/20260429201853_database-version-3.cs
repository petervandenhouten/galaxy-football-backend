using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Layer.Migrations
{
    /// <inheritdoc />
    public partial class databaseversion3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LeagueResults",
                table: "LeagueResults");

            migrationBuilder.RenameTable(
                name: "LeagueResults",
                newName: "league_results");

            migrationBuilder.AddPrimaryKey(
                name: "PK_league_results",
                table: "league_results",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "club_cup_results",
                columns: table => new
                {
                    ClubId = table.Column<Guid>(type: "uuid", nullable: false),
                    CupResultId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_club_cup_results", x => new { x.ClubId, x.CupResultId });
                });

            migrationBuilder.CreateTable(
                name: "club_league_results",
                columns: table => new
                {
                    ClubId = table.Column<Guid>(type: "uuid", nullable: false),
                    LeagueResultId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_club_league_results", x => new { x.ClubId, x.LeagueResultId });
                });

            migrationBuilder.CreateTable(
                name: "league_league_results",
                columns: table => new
                {
                    LeagueId = table.Column<Guid>(type: "uuid", nullable: false),
                    LeagueResultId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_league_league_results", x => new { x.LeagueId, x.LeagueResultId });
                });

            migrationBuilder.CreateTable(
                name: "leagues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Number = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_leagues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "player_club_teams",
                columns: table => new
                {
                    PlayerId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClubId = table.Column<Guid>(type: "uuid", nullable: false),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player_club_teams", x => new { x.PlayerId, x.ClubId, x.TeamId });
                });

            migrationBuilder.CreateTable(
                name: "season_cup_results",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SeasonYear = table.Column<int>(type: "integer", nullable: false),
                    Ranking = table.Column<int>(type: "integer", nullable: false),
                    Result = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_season_cup_results", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "season_league_results",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SeasonYear = table.Column<int>(type: "integer", nullable: false),
                    DivisionLevel = table.Column<int>(type: "integer", nullable: false),
                    DivisionNumber = table.Column<int>(type: "integer", nullable: false),
                    Ranking = table.Column<int>(type: "integer", nullable: false),
                    Result = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_season_league_results", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "club_cup_results");

            migrationBuilder.DropTable(
                name: "club_league_results");

            migrationBuilder.DropTable(
                name: "league_league_results");

            migrationBuilder.DropTable(
                name: "leagues");

            migrationBuilder.DropTable(
                name: "player_club_teams");

            migrationBuilder.DropTable(
                name: "season_cup_results");

            migrationBuilder.DropTable(
                name: "season_league_results");

            migrationBuilder.DropPrimaryKey(
                name: "PK_league_results",
                table: "league_results");

            migrationBuilder.RenameTable(
                name: "league_results",
                newName: "LeagueResults");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeagueResults",
                table: "LeagueResults",
                column: "Id");
        }
    }
}
