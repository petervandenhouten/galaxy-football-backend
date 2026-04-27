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

        // Associative entities
        // public DbSet<UserPlayer> UserPlayers => Set<UserPlayer>();
        // public DbSet<PlayerClub> PlayerClubs => Set<PlayerClub>();
        // public DbSet<ClubTeam> ClubTeams => Set<ClubTeam>();
        // public DbSet<ClubStadium> ClubStadiums => Set<ClubStadium>();
        // public DbSet<ClubSponsor> ClubSponsors => Set<ClubSponsor>();
        // public DbSet<TeamRobot> TeamRobots => Set<TeamRobot>();
        // public DbSet<TeamCompetition> TeamCompetitions => Set<TeamCompetition>();
        // public DbSet<TeamMatchLineup> TeamMatchLineups => Set<TeamMatchLineup>();
        // public DbSet<TeamSavedLineup> TeamSavedLineups => Set<TeamSavedLineup>();
        // public DbSet<AutoCoachClubTeam> AutoCoachClubTeams => Set<AutoCoachClubTeam>();

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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

            // TODO: Add model configuration for associative entities as needed
        }
    }
}