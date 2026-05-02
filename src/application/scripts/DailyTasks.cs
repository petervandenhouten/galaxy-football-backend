using GalaxyFootball.Infrastructure.Database;
using Microsoft.Extensions.Logging;
using GalaxyFootball.Domain.Entities;

namespace GalaxyFootball.Application.Scripts
{
    // Daily tasks:
    // - check what kind of day we have to process
    // - run approproiate scripts for that day (e.g. start new season, start new game, etc.)
    // - progress to the next day

    public class DailyTasks : BaseScript
    {
        private readonly ILogger<DailyTasks> m_logger;
        public DailyTasks(ApplicationDbContext db, ILoggerFactory loggerFactory)
            : base(db, loggerFactory)
        {
            m_logger = loggerFactory.CreateLogger<DailyTasks>();
        }

        public override bool CanRun()
        {
            var game = m_db.Games.FirstOrDefault();
            m_logger.LogInformation("Checking if a daily task can run...");
            m_logger.LogInformation("Game exists: {Exists}", game is not null);
            if ( game is null) return false;
            m_logger.LogInformation("Game paused: {Paused}", game.IsPaused);
            m_logger.LogInformation("Game locked: {Locked}", game.IsLocked);
            m_logger.LogInformation("Game processing: {Processing}", game.IsProcessing);
            m_logger.LogInformation("Game date {Year}-{Day}", game.Year, game.Day);

            var day = m_db.Calendar.FirstOrDefault(c => c.DayIndex == game.Day);
            if (day is null)            
            {
                m_logger.LogError("No calendar entry found for current game day {Day}",game.Day);
            }   

            // Return false if no Game exists in the database
            return !game.IsPaused && !game.IsLocked && !game.IsProcessing && day is not null;
        }

        public override async Task Run()
        {
            var game = m_db.Games.First();
            var day  = m_db.Calendar.First(c => c.DayIndex == game.Day);

            m_logger.LogInformation("Processing daily task for day {Year}-{Day}", game.Year, game.Day);

            // For all days
            await RunScriptByName("HandleTransfers");
            await RunScriptByName("CreateCupMatches"); // for upcoming cup round
            await RunScriptByName("UpdateWeatherConditions"); // for upcoming matches
            await RunScriptByName("UpdateRobotInjuries"); 
            await RunScriptByName("UpdateAutoCoachTeams"); 

            switch (day.DayType )
            {
                case CalendarDayType.Preseason:
                    m_logger.LogInformation("It's aPre-season day...");
                    await RunScriptByName("PreSeasonDay");
                    break;

                case CalendarDayType.DraftEvent:
                    m_logger.LogInformation("It's the Draft day...");
                    await RunScriptByName("PreSeasonDay");
                    await RunScriptByName("DraftEventDay");
                    break;

                case CalendarDayType.CupMatch:
                    m_logger.LogInformation("It's a cup match day.");
                    await RunScriptByName("ProcessCupMatches");
                    await RunScriptByName("EndOfCupCompetition");
                    break;

                case CalendarDayType.LeagueMatch:
                    m_logger.LogInformation("It's a league match day.");
                    await RunScriptByName("ProcessLeagueMatches");
                    await RunScriptByName("EndOfLeagueCompetition");
                    break;

                case CalendarDayType.FastestPlayerEvent:
                    m_logger.LogInformation("It's a FastestPlayerEvent day.");
                    await RunScriptByName("FastestPlayerEvent");
                    break;

                case CalendarDayType.PenaltyCupEvent:
                    m_logger.LogInformation("It's a PenaltyCupEvent day.");
                    await RunScriptByName("PenaltyCupEvent");
                    break;
                    
                case CalendarDayType.FriendlyMatch:
                    m_logger.LogInformation("It's a FriendlyMatch day.");
                    await RunScriptByName("FriendlyMatch");
                    break;

                case CalendarDayType.AfterSeason:
                    m_logger.LogInformation("It's an AfterSeason day.");
                    await RunScriptByName("AfterSeason");
                    break;

                case CalendarDayType.Idle:
                    m_logger.LogInformation("It's an Idle day. No specific script to run.");
                    break;

                default:
                    m_logger.LogWarning("No day type defined.");
                    break;
            }

            // For all days
            await RunScriptByName("PublishNewsPaper");

            progress_to_next_day();    
        }

        private void progress_to_next_day()
        {
            var game = m_db.Games.First();
            game.Day += 1;
            m_db.SaveChanges();
        }    
    }
}