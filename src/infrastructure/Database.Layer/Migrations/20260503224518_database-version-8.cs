using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Layer.Migrations
{
    /// <inheritdoc />
    public partial class databaseversion8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Stadiums",
                table: "Stadiums");

            migrationBuilder.RenameTable(
                name: "Stadiums",
                newName: "stadiums");

            migrationBuilder.AddColumn<int>(
                name: "TeamIndex",
                table: "team_competitions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "RobotCareerStatistics",
                table: "robots",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RobotHistory",
                table: "robots",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RobotSeasonStatistics",
                table: "robots",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Points",
                table: "league_results",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PreviousRanking",
                table: "league_results",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Ranking",
                table: "league_results",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_stadiums",
                table: "stadiums",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "club_history",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    ClubHistoryEvent = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_club_history", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "matches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Day = table.Column<int>(type: "integer", nullable: false),
                    TeamHomeIndex = table.Column<int>(type: "integer", nullable: false),
                    TeamAwayIndex = table.Column<int>(type: "integer", nullable: false),
                    TeamHomeId = table.Column<Guid>(type: "uuid", nullable: false),
                    TeamAwayId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScoreHome = table.Column<int>(type: "integer", nullable: false),
                    ScoreAway = table.Column<int>(type: "integer", nullable: false),
                    CompetitionType = table.Column<int>(type: "integer", nullable: false),
                    CompetitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CompetitionRound = table.Column<int>(type: "integer", nullable: false),
                    Stadium = table.Column<Guid>(type: "uuid", nullable: false),
                    Weather = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_matches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "robot_history",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    RobotHistoryEvent = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_robot_history", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RobotStatistics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GamePlayed = table.Column<int>(type: "integer", nullable: false),
                    Goals = table.Column<int>(type: "integer", nullable: false),
                    Fouls = table.Column<int>(type: "integer", nullable: false),
                    Interceptions = table.Column<int>(type: "integer", nullable: false),
                    Assists = table.Column<int>(type: "integer", nullable: false),
                    YellowCards = table.Column<int>(type: "integer", nullable: false),
                    RedCards = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RobotStatistics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "weather_conditions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Temperature = table.Column<int>(type: "integer", nullable: false),
                    Rain = table.Column<int>(type: "integer", nullable: false),
                    Wind = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_weather_conditions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "robot_career_statistics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_robot_career_statistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_robot_career_statistics_RobotStatistics_Id",
                        column: x => x.Id,
                        principalTable: "RobotStatistics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "robot_season_statistics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Season = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_robot_season_statistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_robot_season_statistics_RobotStatistics_Id",
                        column: x => x.Id,
                        principalTable: "RobotStatistics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "club_history");

            migrationBuilder.DropTable(
                name: "matches");

            migrationBuilder.DropTable(
                name: "robot_career_statistics");

            migrationBuilder.DropTable(
                name: "robot_history");

            migrationBuilder.DropTable(
                name: "robot_season_statistics");

            migrationBuilder.DropTable(
                name: "weather_conditions");

            migrationBuilder.DropTable(
                name: "RobotStatistics");

            migrationBuilder.DropPrimaryKey(
                name: "PK_stadiums",
                table: "stadiums");

            migrationBuilder.DropColumn(
                name: "TeamIndex",
                table: "team_competitions");

            migrationBuilder.DropColumn(
                name: "RobotCareerStatistics",
                table: "robots");

            migrationBuilder.DropColumn(
                name: "RobotHistory",
                table: "robots");

            migrationBuilder.DropColumn(
                name: "RobotSeasonStatistics",
                table: "robots");

            migrationBuilder.DropColumn(
                name: "Points",
                table: "league_results");

            migrationBuilder.DropColumn(
                name: "PreviousRanking",
                table: "league_results");

            migrationBuilder.DropColumn(
                name: "Ranking",
                table: "league_results");

            migrationBuilder.RenameTable(
                name: "stadiums",
                newName: "Stadiums");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Stadiums",
                table: "Stadiums",
                column: "Id");
        }
    }
}
