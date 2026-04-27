using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Layer.Migrations
{
    /// <inheritdoc />
    public partial class ClubTeamLeagueRobotAutoCoach : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "autocoach_club_teams",
                columns: table => new
                {
                    AutoCoachId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClubId = table.Column<Guid>(type: "uuid", nullable: false),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_autocoach_club_teams", x => new { x.AutoCoachId, x.ClubId, x.TeamId });
                });

            migrationBuilder.CreateTable(
                name: "AutoCoaches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    PreferredLineupPickStrategy = table.Column<int>(type: "integer", nullable: false),
                    PreferredLineUpFormation = table.Column<int>(type: "integer", nullable: false),
                    LineupFormationSelectionMethod = table.Column<int>(type: "integer", nullable: false),
                    LineupPickStrategySelectionMethod = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutoCoaches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "club_sponsors",
                columns: table => new
                {
                    ClubId = table.Column<Guid>(type: "uuid", nullable: false),
                    SponsorId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_club_sponsors", x => new { x.ClubId, x.SponsorId });
                });

            migrationBuilder.CreateTable(
                name: "club_stadiums",
                columns: table => new
                {
                    ClubId = table.Column<Guid>(type: "uuid", nullable: false),
                    StadiumId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_club_stadiums", x => new { x.ClubId, x.StadiumId });
                });

            migrationBuilder.CreateTable(
                name: "club_teams",
                columns: table => new
                {
                    ClubId = table.Column<Guid>(type: "uuid", nullable: false),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_club_teams", x => new { x.ClubId, x.TeamId });
                });

            migrationBuilder.CreateTable(
                name: "LeagueResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    CompetitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    HomePlayed = table.Column<int>(type: "integer", nullable: false),
                    HomeWins = table.Column<int>(type: "integer", nullable: false),
                    HomeDraws = table.Column<int>(type: "integer", nullable: false),
                    HomeLosses = table.Column<int>(type: "integer", nullable: false),
                    HomeGoalsFor = table.Column<int>(type: "integer", nullable: false),
                    HomeGoalsAgainst = table.Column<int>(type: "integer", nullable: false),
                    AwayPlayed = table.Column<int>(type: "integer", nullable: false),
                    AwayWins = table.Column<int>(type: "integer", nullable: false),
                    AwayDraws = table.Column<int>(type: "integer", nullable: false),
                    AwayLosses = table.Column<int>(type: "integer", nullable: false),
                    AwayGoalsFor = table.Column<int>(type: "integer", nullable: false),
                    AwayGoalsAgainst = table.Column<int>(type: "integer", nullable: false),
                    WinningStreak = table.Column<int>(type: "integer", nullable: false),
                    LosingStreak = table.Column<int>(type: "integer", nullable: false),
                    MatchResults = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeagueResults", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "robot_batteries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Capacity = table.Column<double>(type: "double precision", nullable: false),
                    DischargeRate = table.Column<double>(type: "double precision", nullable: false),
                    ConversionEfficiency = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_robot_batteries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "robot_bodies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Mass = table.Column<double>(type: "double precision", nullable: false),
                    Traction = table.Column<double>(type: "double precision", nullable: false),
                    RotationResistance = table.Column<double>(type: "double precision", nullable: false),
                    ShootingPower = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_robot_bodies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "robot_brains",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BrainType = table.Column<int>(type: "integer", nullable: false),
                    ReactionTime = table.Column<double>(type: "double precision", nullable: false),
                    ViewRange = table.Column<double>(type: "double precision", nullable: false),
                    Anticipation = table.Column<double>(type: "double precision", nullable: false),
                    ShootingAccuracy = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_robot_brains", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "robot_motors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MaxSpeed = table.Column<double>(type: "double precision", nullable: false),
                    Acceleration = table.Column<double>(type: "double precision", nullable: false),
                    Braking = table.Column<double>(type: "double precision", nullable: false),
                    MaxRotationSpeed = table.Column<double>(type: "double precision", nullable: false),
                    RotationAcceleration = table.Column<double>(type: "double precision", nullable: false),
                    HeatGenerationRate = table.Column<double>(type: "double precision", nullable: false),
                    CoolingRate = table.Column<double>(type: "double precision", nullable: false),
                    MaxTemperature = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_robot_motors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "robots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: true),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    Planet = table.Column<string>(type: "text", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Face = table.Column<string>(type: "text", nullable: false),
                    BrainId = table.Column<Guid>(type: "uuid", nullable: false),
                    BodyId = table.Column<Guid>(type: "uuid", nullable: false),
                    MotorId = table.Column<Guid>(type: "uuid", nullable: false),
                    BatteryId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentMentalShape = table.Column<double>(type: "double precision", nullable: false),
                    CurrentCondition = table.Column<double>(type: "double precision", nullable: false),
                    WearSpeed = table.Column<double>(type: "double precision", nullable: false),
                    RecoveryRate = table.Column<double>(type: "double precision", nullable: false),
                    Fatigue = table.Column<double>(type: "double precision", nullable: false),
                    MatchesBannedRemaining = table.Column<int>(type: "integer", nullable: false),
                    MatchesUnavailableRemaining = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_robots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stadiums",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Capacity = table.Column<int>(type: "integer", nullable: false),
                    PitchType = table.Column<string>(type: "text", nullable: false),
                    Planet = table.Column<string>(type: "text", nullable: false),
                    WeatherTendencies = table.Column<string>(type: "text", nullable: false),
                    Facilities = table.Column<string>(type: "text", nullable: false),
                    TicketPrice = table.Column<int>(type: "integer", nullable: false),
                    MaintenanceCost = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stadiums", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "team_competitions",
                columns: table => new
                {
                    TeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    CompetitionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_team_competitions", x => new { x.TeamId, x.CompetitionId });
                });

            migrationBuilder.CreateTable(
                name: "team_match_lineups",
                columns: table => new
                {
                    TeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    MatchLineupId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_team_match_lineups", x => new { x.TeamId, x.MatchLineupId });
                });

            migrationBuilder.CreateTable(
                name: "team_robots",
                columns: table => new
                {
                    TeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    RobotId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_team_robots", x => new { x.TeamId, x.RobotId });
                });

            migrationBuilder.CreateTable(
                name: "team_saved_lineups",
                columns: table => new
                {
                    TeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    SavedLineupId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_team_saved_lineups", x => new { x.TeamId, x.SavedLineupId });
                });

            migrationBuilder.CreateTable(
                name: "user_players",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlayerId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_players", x => new { x.UserId, x.PlayerId });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "autocoach_club_teams");

            migrationBuilder.DropTable(
                name: "AutoCoaches");

            migrationBuilder.DropTable(
                name: "club_sponsors");

            migrationBuilder.DropTable(
                name: "club_stadiums");

            migrationBuilder.DropTable(
                name: "club_teams");

            migrationBuilder.DropTable(
                name: "LeagueResults");

            migrationBuilder.DropTable(
                name: "robot_batteries");

            migrationBuilder.DropTable(
                name: "robot_bodies");

            migrationBuilder.DropTable(
                name: "robot_brains");

            migrationBuilder.DropTable(
                name: "robot_motors");

            migrationBuilder.DropTable(
                name: "robots");

            migrationBuilder.DropTable(
                name: "Stadiums");

            migrationBuilder.DropTable(
                name: "team_competitions");

            migrationBuilder.DropTable(
                name: "team_match_lineups");

            migrationBuilder.DropTable(
                name: "team_robots");

            migrationBuilder.DropTable(
                name: "team_saved_lineups");

            migrationBuilder.DropTable(
                name: "user_players");
        }
    }
}
