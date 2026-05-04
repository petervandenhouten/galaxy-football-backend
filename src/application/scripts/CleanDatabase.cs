using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GalaxyFootball.Infrastructure.Database;

namespace GalaxyFootball.Application.Scripts
{
    /// <summary>
    /// Script to clean up the database. Implement your cleanup logic here.
    /// </summary>
    public class CleanDatabase : BaseScript
    {
        private readonly ILogger<CleanDatabase> m_logger;
        public CleanDatabase(ApplicationDbContext db, ILoggerFactory loggerFactory)
            : base(db, loggerFactory)
        {
            m_logger = loggerFactory.CreateLogger<CleanDatabase>();
        }

        public override async Task<Task> Run()
        {
            m_logger.LogInformation("Running CleanDatabase script...");
            // TODO: Add your database cleanup logic here
            // Example: Remove old logs, orphaned records, etc.
            await Task.CompletedTask;
            return Task.CompletedTask;
        }

        public override bool CanRun()
        {
            return true;
        }
    }
}
