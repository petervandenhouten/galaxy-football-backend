using Microsoft.EntityFrameworkCore;
using GalaxyFootball.Infrastructure.Database;


public static class AdvisoryLockConstants
{
    public const int JobLockKey = 12345;
}

public class JobService
{
    private readonly ApplicationDbContext m_db;
    private readonly ILogger<JobService> m_logger;

    public JobService(ApplicationDbContext db, ILogger<JobService> logger)
    {
        m_db = db;
        m_logger = logger;
    }

    public async Task RunIfNeeded()
    {
        using var transaction = await m_db.Database.BeginTransactionAsync();
        // Lock the single Game row for update
        var game = await m_db.Games.FromSqlRaw("SELECT * FROM \"games\" FOR UPDATE").FirstOrDefaultAsync();
        if (game == null)
        {
            m_logger.LogWarning("No game state found. Cannot run job.");
            return;
        }

        if (game.IsProcessing)
        {
            m_logger.LogInformation("Job is already running elsewhere (Game state is processing).");
            return;
        }

        // Set lock flags
        game.IsProcessing = true;
        await m_db.SaveChangesAsync();

        try
        {
            await RunJob();
            m_logger.LogInformation("Daily job executed successfully");
        }
        finally
        {
            // Release lock flags
            game.IsProcessing = false;
            await m_db.SaveChangesAsync();
            await transaction.CommitAsync();
        }
    }

    public async Task ForceRun()
    {
        m_logger.LogInformation("Force running daily job");

        await RunJob();

        //var task = await m_db.SystemTasks.FindAsync("daily_job");
        //task.LastRun = DateTime.UtcNow;

        // await m_db.SaveChangesAsync();
    }

    private async Task RunJob()
    {
        // 🔥 YOUR ACTUAL BATCH LOGIC HERE
        m_logger.LogInformation("Running batch logic...");

        // Example:
        // - reset daily rewards
        // - cleanup expired data
        // - recalculate leaderboard
    }
}