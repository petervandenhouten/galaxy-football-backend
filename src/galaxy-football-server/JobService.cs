public class JobService
{
    //private readonly AppDbContext _db;
    private readonly ILogger<JobService> _logger;

    public JobService(/*AppDbContext db,*/ ILogger<JobService> logger)
    {
      //  _db = db;
        _logger = logger;
    }

    public async Task RunIfNeeded()
    {
        // await _db.Database.ExecuteSqlRawAsync("SELECT pg_advisory_lock(12345)");

        try
        {
            //var task = await _db.SystemTasks.FindAsync("daily_job");
            var today = DateTime.UtcNow.Date;

            //if (task.LastRun.Date >= today)
            //{
            //    _logger.LogInformation("Daily job already ran today");
            //    return;
            //}

             await RunJob();

             //task.LastRun = DateTime.UtcNow;
        //     await _db.SaveChangesAsync();

             _logger.LogInformation("Daily job executed successfully");
        }
        finally
        {
            // await _db.Database.ExecuteSqlRawAsync("SELECT pg_advisory_unlock(12345)");
        }
    }

    public async Task ForceRun()
    {
        _logger.LogInformation("Force running daily job");

        await RunJob();

        //var task = await _db.SystemTasks.FindAsync("daily_job");
        //task.LastRun = DateTime.UtcNow;

        // await _db.SaveChangesAsync();
    }

    private async Task RunJob()
    {
        // 🔥 YOUR ACTUAL BATCH LOGIC HERE
        _logger.LogInformation("Running batch logic...");

        // Example:
        // - reset daily rewards
        // - cleanup expired data
        // - recalculate leaderboard
    }
}