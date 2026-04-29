using Microsoft.EntityFrameworkCore;
using GalaxyFootball.Domain.Entities;

namespace GalaxyFootball.Infrastructure.Database
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Game> Games => Set<Game>();
        public DbSet<Club> Clubs => Set<Club>();
        public DbSet<Player> Players => Set<Player>();
        public DbSet<Team> Teams => Set<Team>();
        public DbSet<Robot> Robots => Set<Robot>();
        public DbSet<RobotBrain> RobotBrains => Set<RobotBrain>();
        public DbSet<RobotBody> RobotBodies => Set<RobotBody>();
        public DbSet<RobotBattery> RobotBatteries => Set<RobotBattery>();
        public DbSet<RobotMotor> RobotMotors => Set<RobotMotor>();
        public DbSet<AutoCoach> AutoCoaches => Set<AutoCoach>();
        public DbSet<Stadium> Stadiums => Set<Stadium>();
        public DbSet<League> Leagues => Set<League>();
        public DbSet<LeagueResult> LeagueResults => Set<LeagueResult>();
        public DbSet<SeasonLeagueResult> SeasonLeagueResults => Set<SeasonLeagueResult>();
        public DbSet<SeasonCupResult> SeasonCupResults => Set<SeasonCupResult>();
        public DbSet<ClubLeagueResult> ClubLeagueResults => Set<ClubLeagueResult>();
        public DbSet<ClubCupResult> ClubCupResults => Set<ClubCupResult>();
        public DbSet<LeagueLeagueResult> LeagueLeagueResults => Set<LeagueLeagueResult>();

        // Associative entities
        public DbSet<UserPlayer> UserPlayers => Set<UserPlayer>();
        public DbSet<PlayerClubTeam> PlayerClubTeams => Set<PlayerClubTeam>();
        public DbSet<ClubTeam> ClubTeams => Set<ClubTeam>();
        public DbSet<ClubStadium> ClubStadiums => Set<ClubStadium>();
        public DbSet<ClubSponsor> ClubSponsors => Set<ClubSponsor>();
        public DbSet<TeamRobot> TeamRobots => Set<TeamRobot>();
        public DbSet<TeamCompetition> TeamCompetitions => Set<TeamCompetition>();
        public DbSet<TeamMatchLineup> TeamMatchLineups => Set<TeamMatchLineup>();
        public DbSet<TeamSavedLineup> TeamSavedLineups => Set<TeamSavedLineup>();
        public DbSet<AutoCoachClubTeam> AutoCoachClubTeams => Set<AutoCoachClubTeam>();

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<League>(entity =>
            {
                entity.ToTable("leagues");
                entity.HasKey(l => l.Id);
                entity.Property(l => l.Level).IsRequired();
                entity.Property(l => l.Number).IsRequired();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Password).IsRequired();
                entity.Property(u => u.CreatedAt).IsRequired();
                entity.HasIndex(u => u.Email).IsUnique();
            });

            modelBuilder.Entity<Game>(entity =>
            {
                entity.ToTable("games");
                entity.HasKey(g => g.Id);
                entity.Property(g => g.Id).ValueGeneratedNever();
                entity.Property(g => g.Year).IsRequired();
                entity.Property(g => g.Day).IsRequired();
                entity.Property(g => g.DaysBetweenGames).IsRequired();
                entity.Property(g => g.MaxLeagueRounds).IsRequired();
                entity.Property(g => g.CurrentLeagueRound).IsRequired();
                entity.Property(g => g.MaxCupRounds).IsRequired();
                entity.Property(g => g.CurrentCupRound).IsRequired();
                entity.Property(g => g.NumberOfTeamsInLeague).IsRequired();
                entity.Property(g => g.IsPaused).IsRequired();
                entity.Property(g => g.IsProcessing).IsRequired();
                entity.Property(g => g.IsLocked).IsRequired();
                entity.Property(g => g.DatabaseVersion).IsRequired();
                entity.Property(g => g.GameVersion).IsRequired();
            });

            modelBuilder.Entity<Club>(entity =>
            {
                entity.ToTable("clubs");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired();
                entity.Property(c => c.PlanetName).IsRequired();
                entity.Property(c => c.FoundationYear).IsRequired();
                entity.Property(c => c.PrimaryColor).IsRequired();
                entity.Property(c => c.SecondaryColor).IsRequired();
                entity.Property(c => c.TertiaryColor).IsRequired();
                entity.Property(c => c.UniformStyle).IsRequired();
                entity.Property(c => c.LogoReference).IsRequired();
                entity.Property(c => c.Balance).IsRequired();
                entity.Property(c => c.LastIncome).IsRequired();
                entity.Property(c => c.LastExpenses).IsRequired();
                entity.Property(c => c.FanCount).IsRequired();
                entity.Property(c => c.FanHappiness).IsRequired();
                entity.Property(c => c.FanExpectations).IsRequired();
                entity.Property(c => c.ClubRankingPoints).IsRequired();
                entity.Property(c => c.SeasonGamesWon).IsRequired();
                entity.Property(c => c.SeasonGamesLost).IsRequired();
                entity.Property(c => c.SeasonGamesDrawn).IsRequired();
                entity.Property(c => c.SeasonGoalsScored).IsRequired();
                entity.Property(c => c.SeasonGoalsConceded).IsRequired();
                entity.Property(c => c.WinningStreak).IsRequired();
                entity.Property(c => c.LosingStreak).IsRequired();
                entity.Property(c => c.AllTimeGamesWon).IsRequired();
                entity.Property(c => c.AllTimeGamesLost).IsRequired();
                entity.Property(c => c.AllTimeGamesDrawn).IsRequired();
                entity.Property(c => c.AllTimeGoalsScored).IsRequired();
                entity.Property(c => c.AllTimeGoalsConceded).IsRequired();
            });

            modelBuilder.Entity<Player>(entity =>
            {
                entity.ToTable("players");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.FirstName).IsRequired();
                entity.Property(p => p.LastName).IsRequired();
            });

            modelBuilder.Entity<Team>(entity =>
            {
                entity.ToTable("teams");
                entity.HasKey(t => t.Id);
            });

            modelBuilder.Entity<Robot>(entity =>
            {
                entity.ToTable("robots");
                entity.HasKey(r => r.Id);
                entity.Property(r => r.FirstName).IsRequired();
                entity.Property(r => r.LastName).IsRequired();
                entity.Property(r => r.Planet).IsRequired();
                entity.Property(r => r.CreationYear).IsRequired();
                entity.Property(r => r.CreationDay).IsRequired();
                entity.Property(r => r.Face).IsRequired();
                entity.Property(r => r.BrainId).IsRequired();
                entity.Property(r => r.BodyId).IsRequired();
                entity.Property(r => r.MotorId).IsRequired();
                entity.Property(r => r.BatteryId).IsRequired();
            });

            modelBuilder.Entity<RobotBrain>(entity =>
            {
                entity.ToTable("robot_brains");
                entity.HasKey(b => b.Id);
                entity.Property(b => b.BrainType).IsRequired();
                entity.Property(b => b.ReactionTime).IsRequired();
                entity.Property(b => b.ViewRange).IsRequired();
                entity.Property(b => b.Anticipation).IsRequired();
                entity.Property(b => b.ShootingAccuracy).IsRequired();
            });

            modelBuilder.Entity<RobotBody>(entity =>
            {
                entity.ToTable("robot_bodies");
                entity.HasKey(b => b.Id);
                entity.Property(b => b.Mass).IsRequired();
                entity.Property(b => b.Traction).IsRequired();
                entity.Property(b => b.RotationResistance).IsRequired();
                entity.Property(b => b.ShootingPower).IsRequired();
            });

            modelBuilder.Entity<RobotBattery>(entity =>
            {
                entity.ToTable("robot_batteries");
                entity.HasKey(b => b.Id);
                entity.Property(b => b.Capacity).IsRequired();
                entity.Property(b => b.DischargeRate).IsRequired();
                entity.Property(b => b.ConversionEfficiency).IsRequired();
            });

            modelBuilder.Entity<RobotMotor>(entity =>
            {
                entity.ToTable("robot_motors");
                entity.HasKey(m => m.Id);
                entity.Property(m => m.MaxSpeed).IsRequired();
                entity.Property(m => m.Acceleration).IsRequired();
                entity.Property(m => m.Braking).IsRequired();
                entity.Property(m => m.MaxRotationSpeed).IsRequired();
                entity.Property(m => m.RotationAcceleration).IsRequired();
                entity.Property(m => m.HeatGenerationRate).IsRequired();
                entity.Property(m => m.CoolingRate).IsRequired();
                entity.Property(m => m.MaxTemperature).IsRequired();
            });

            modelBuilder.Entity<AutoCoachClubTeam>(entity =>
            {
                entity.ToTable("autocoach_club_teams");
                entity.HasKey(e => new { e.AutoCoachId, e.ClubId, e.TeamId });
            });

            modelBuilder.Entity<ClubSponsor>(entity =>
            {
                entity.ToTable("club_sponsors");
                entity.HasKey(e => new { e.ClubId, e.SponsorId });
            });

            modelBuilder.Entity<UserPlayer>(entity =>
            {
                entity.ToTable("user_players");
                entity.HasKey(e => new { e.UserId, e.PlayerId });
            });

            modelBuilder.Entity<ClubTeam>(entity =>
            {
                entity.ToTable("club_teams");
                entity.HasKey(e => new { e.ClubId, e.TeamId });
            });

            modelBuilder.Entity<ClubStadium>(entity =>
            {
                entity.ToTable("club_stadiums");
                entity.HasKey(e => new { e.ClubId, e.StadiumId });
            });

            modelBuilder.Entity<ClubSponsor>(entity =>
            {
                entity.ToTable("club_sponsors");
                entity.HasKey(e => new { e.ClubId, e.SponsorId });
            });

            modelBuilder.Entity<TeamRobot>(entity =>
            {
                entity.ToTable("team_robots");
                entity.HasKey(e => new { e.TeamId, e.RobotId });
            });

            modelBuilder.Entity<TeamCompetition>(entity =>
            {
                entity.ToTable("team_competitions");
                entity.HasKey(e => new { e.TeamId, e.CompetitionId });
            });

            modelBuilder.Entity<TeamMatchLineup>(entity =>
            {
                entity.ToTable("team_match_lineups");
                entity.HasKey(e => new { e.TeamId, e.MatchLineupId });
            });

            modelBuilder.Entity<TeamSavedLineup>(entity =>
            {
                entity.ToTable("team_saved_lineups");
                entity.HasKey(e => new { e.TeamId, e.SavedLineupId });
            });

            modelBuilder.Entity<PlayerClubTeam>(entity =>
            {
                entity.ToTable("player_club_teams");
                entity.HasKey(e => new { e.PlayerId, e.ClubId, e.TeamId });
            });

            modelBuilder.Entity<LeagueResult>(entity =>
            {
                entity.ToTable("league_results");
                entity.HasKey(lr => lr.Id);
                entity.Property(lr => lr.TeamId).IsRequired();
                entity.Property(lr => lr.CompetitionId).IsRequired();
                entity.Property(lr => lr.HomePlayed).IsRequired();
                entity.Property(lr => lr.HomeWins).IsRequired();
                entity.Property(lr => lr.HomeDraws).IsRequired();
                entity.Property(lr => lr.HomeLosses).IsRequired();
                entity.Property(lr => lr.HomeGoalsFor).IsRequired();
                entity.Property(lr => lr.HomeGoalsAgainst).IsRequired();
                entity.Property(lr => lr.AwayPlayed).IsRequired();
                entity.Property(lr => lr.AwayWins).IsRequired();
                entity.Property(lr => lr.AwayDraws).IsRequired();
                entity.Property(lr => lr.AwayLosses).IsRequired();
                entity.Property(lr => lr.AwayGoalsFor).IsRequired();
                entity.Property(lr => lr.AwayGoalsAgainst).IsRequired();
                entity.Property(lr => lr.WinningStreak).IsRequired();
                entity.Property(lr => lr.LosingStreak).IsRequired();
                entity.Property(lr => lr.MatchResults).IsRequired();
            });

            modelBuilder.Entity<SeasonLeagueResult>(entity =>
            {
                entity.ToTable("season_league_results");
                entity.HasKey(slr => slr.Id);
                entity.Property(slr => slr.SeasonYear).IsRequired();
                entity.Property(slr => slr.DivisionLevel).IsRequired();
                entity.Property(slr => slr.DivisionNumber).IsRequired();
                entity.Property(slr => slr.Ranking).IsRequired();
                entity.Property(slr => slr.Result).IsRequired();
            });

            modelBuilder.Entity<SeasonCupResult>(entity =>
            {
                entity.ToTable("season_cup_results");
                entity.HasKey(scr => scr.Id);
                entity.Property(scr => scr.SeasonYear).IsRequired();
                entity.Property(scr => scr.Ranking).IsRequired();
                entity.Property(scr => scr.Result).IsRequired();
            });

                        modelBuilder.Entity<PlayerClubTeam>(entity =>
            {
                entity.ToTable("player_club_teams");
                entity.HasKey(e => new { e.PlayerId, e.ClubId, e.TeamId });
            });

            modelBuilder.Entity<ClubTeam>(entity =>
            {
                entity.ToTable("club_teams");
                entity.HasKey(e => new { e.ClubId, e.TeamId });
            });

            modelBuilder.Entity<ClubStadium>(entity =>
            {
                entity.ToTable("club_stadiums");
                entity.HasKey(e => new { e.ClubId, e.StadiumId });
            });

            modelBuilder.Entity<ClubSponsor>(entity =>
            {
                entity.ToTable("club_sponsors");
                entity.HasKey(e => new { e.ClubId, e.SponsorId });
            });

            modelBuilder.Entity<ClubLeagueResult>(entity =>
            {
                entity.ToTable("club_league_results");
                entity.HasKey(e => new { e.ClubId, e.LeagueResultId });
            });

            modelBuilder.Entity<ClubCupResult>(entity =>
            {
                entity.ToTable("club_cup_results");
                entity.HasKey(e => new { e.ClubId, e.CupResultId });
            });

            modelBuilder.Entity<TeamRobot>(entity =>
            {
                entity.ToTable("team_robots");
                entity.HasKey(e => new { e.TeamId, e.RobotId });
            });

            modelBuilder.Entity<TeamCompetition>(entity =>
            {
                entity.ToTable("team_competitions");
                entity.HasKey(e => new { e.TeamId, e.CompetitionId });
            });

            modelBuilder.Entity<TeamMatchLineup>(entity =>
            {
                entity.ToTable("team_match_lineups");
                entity.HasKey(e => new { e.TeamId, e.MatchLineupId });
            });

            modelBuilder.Entity<TeamSavedLineup>(entity =>
            {
                entity.ToTable("team_saved_lineups");
                entity.HasKey(e => new { e.TeamId, e.SavedLineupId });
            });

            modelBuilder.Entity<LeagueLeagueResult>(entity =>
            {
                entity.ToTable("league_league_results");
                entity.HasKey(e => new { e.LeagueId, e.LeagueResultId });
            });
        }
    }
}