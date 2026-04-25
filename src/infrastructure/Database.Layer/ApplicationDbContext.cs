using Microsoft.EntityFrameworkCore;
using GalaxyFootball.Domain.Entities;

namespace GalaxyFootball.Infrastructure.Database
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Game> Games => Set<Game>();

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
                entity.Property(g => g.IsPaused).IsRequired();
                entity.Property(g => g.IsLocked).IsRequired();
                entity.Property(g => g.GameDate).IsRequired();
                entity.Property(g => g.IsBatchProcessing).IsRequired();
                entity.Property(g => g.DatabaseVersion).IsRequired();
                // Optionally, enforce only one row by convention (Id = fixed value)
            });
        }
    }
}