using GalaxyFootball.Infrastructure.Database;
using Microsoft.Extensions.Logging;

namespace GalaxyFootball.Application.Scripts
{
    /// <summary>
    /// Script to start a new season. Should be called as part of starting a new game or when a new season begins.
    /// </summary>
    public class StartNewSeason : BaseScript
    {
        private readonly ILogger<StartNewSeason> m_logger;

        public StartNewSeason(ApplicationDbContext db, ILoggerFactory loggerFactory)
            : base(db, loggerFactory)
        {
            m_logger = loggerFactory.CreateLogger<StartNewSeason>();
        }

        public override /* async */ Task Run()
        {
            // Implement the logic to start a new season
            m_logger.LogInformation("Starting a new season...");
            // TODO: Add season initialization logic here
            return Task.CompletedTask;
        }

        public override bool CanRun()
        {
            // Add logic to determine if a new season can be started
            return true;
        }
    }
}
