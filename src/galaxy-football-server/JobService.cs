using Microsoft.EntityFrameworkCore;
using GalaxyFootball.Infrastructure.Database;

public static class AdvisoryLockConstants
{
    public const int JobLockKey = 12345;
}

// For admin endpoints, they should bypass the IsProcessing check and locking, 
// allowing a forced run even if a job is already running or stuck. 
//It should log this override clearly.

public class JobService
{
    private readonly ApplicationDbContext m_db;
    private readonly ILogger<JobService> m_logger;
    private readonly ScriptRunner m_scriptRunner;

    public JobService(ApplicationDbContext db, ILogger<JobService> logger, ScriptRunner scriptRunner)
    {
        m_db = db;
        m_logger = logger;
        m_scriptRunner = scriptRunner;
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
            await RunDailyJob();
            m_logger.LogInformation("Daily job executed successfully");
        }
        finally
        {
            // Release lock flags
            game.IsProcessing = false;
            await m_db.SaveChangesAsync();
            await transaction.CommitAsync();

            // Note: The SaveChangesAsync at the end of RunIfNeeded is only for the
            // IsProcessing flag and transaction commit, not for the business logic.
            // So, always save changes inside RunDailyJob when you modify the database.
        }
    }

    // Admin task to force run the daily job, bypassing locks and checks
    public async Task ForceRun()
    {
        m_logger.LogInformation("Forcing running daily job by Administrator.");

        try
        {
            await RunDailyJob();
            m_logger.LogInformation("Daily job executed successfully");
        }
        catch (Exception ex)
        {
            m_logger.LogError(ex, "Error while forcing daily job. Message: {Message}", ex.Message);
            throw; // rethrow to let the caller handle it (e.g., return 500)
        }
    }

    private async Task RunDailyJob()
    {
        // 🔥 YOUR ACTUAL BATCH LOGIC HERE
        m_logger.LogInformation("Running batch logic for daily job...");

        await m_scriptRunner.RunScriptByName("DailyTasks");
        await m_scriptRunner.RunScriptByName("CleanDatabase");
    }

    // Admin task to force start a new game/season, bypassing locks and checks
    public async Task ForceStartNewGame()
    {
        m_logger.LogInformation("Force starting new game by Administrator.");
        await m_scriptRunner.RunScriptByName("StartNewGame");
    }
}