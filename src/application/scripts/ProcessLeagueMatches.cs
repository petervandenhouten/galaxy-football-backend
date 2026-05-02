using Microsoft.Extensions.Logging;

using GalaxyFootball.Infrastructure.Database;
using GalaxyFootball.Domain.Entities;

namespace GalaxyFootball.Application.Scripts
{
    public class ProcessLeagueMatches : BaseScript
    {
        private readonly ILogger<ProcessLeagueMatches> m_logger;
        public ProcessLeagueMatches(ApplicationDbContext db, ILoggerFactory loggerFactory)
            : base(db, loggerFactory)
        {
            m_logger = loggerFactory.CreateLogger<ProcessLeagueMatches>();
        }

        public override bool CanRun()
        {
            var game = m_db.Games.FirstOrDefault();
            if ( game is not null )
            {
                var day = m_db.Calendar.FirstOrDefault(c => c.DayIndex == game.Day);
                return day is not null && day.DayType == CalendarDayType.LeagueMatch;
            }
            return false;
        }

        public override async Task Run()
        {
            int current_round = m_db.Games.First().CurrentLeagueRound;

            m_logger.LogInformation("Processing league matches. Current league round: {Round}", current_round);

            // Get all matches scheduled for today
            var matches = m_db.Matches.Where(m => m.CompetitionRound == current_round && m.CompetitionType == CalendarDayType.LeagueMatch).ToList();
            m_logger.LogInformation("Number of matches: {Count}", matches.Count);

            // Simulate each match in parallel threads (no database writes)
            // The engine will only read from the database and write results to external storage (e.g., Cloudflare)
            var matchTasks = matches.Select(match => SimulateMatchAsync(match)).ToList();
            var matchResults = await Task.WhenAll(matchTasks);

            // Update results, team stats, and league standings in single thread (main thread only)
            foreach (var result in matchResults)
            {
                if (result != null)
                {
                    m_logger.LogInformation("Updating results for match ID: {MatchID}", result.Id);
                    UpdateMatchResultsInDatabase(result);
                }
            }

            // Save all match results to database in one operation
            await m_db.SaveChangesAsync();

            // update_leagues_standings();
            // update_club_statistics();
            // update_robot_statistics();

            m_logger.LogInformation("League matches for round {Round} completed", current_round);
        }

        // Todo: We could have a match input class here that only contains the necessary info for the engine to simulate the match,
        // and a match result class that contains the results from the engine.
        // This way we can decouple the database entities from the engine input/output and  avoid accidental database writes in the simulation step.
        private async Task<Match> SimulateMatchAsync(Match match)
        {
            // Simulate match in parallel thread - NO database writes here
            m_logger.LogInformation("Simulating match ID: {MatchID}", match.Id);
            
            // TODO: Call engine to simulate match and store results in external storage
            // Return match object with updated scores (but don't save to database)

            match.ScoreHome = new Random().Next(0, 5); // Placeholder for actual simulation result
            match.ScoreAway = new Random().Next(0, 5); // Placeholder for actual
                        
            return await Task.FromResult(match);
        }

        private void UpdateMatchResultsInDatabase(Match match)
        {
            // Update match results, team stats, league standings, etc.
            // This is called only in the main thread after all simulations are complete
            m_logger.LogInformation("Updating database for match: {MatchID}", match.Id);
            
            // TODO: Update team stats, league results, handle special events, etc.
            var matchInDb = m_db.Matches.FirstOrDefault(m => m.Id == match.Id);
            if (matchInDb != null)
            {
                matchInDb.ScoreHome = match.ScoreHome;
                matchInDb.ScoreAway = match.ScoreAway;
                // Update other fields as needed
            }
        }
    }
}