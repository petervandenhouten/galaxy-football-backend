using Microsoft.Extensions.Logging;

using GalaxyFootball.Infrastructure.Database;
using GalaxyFootball.Domain.Entities;
using GalaxyFootball.Application.MatchEngine;
using GalaxyFootball.Application.Interfaces;
using System.Security.Cryptography;

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
            
            // Pre-load all clubs to avoid nested queries during parallel execution
            var allClubs = m_db.Clubs.ToList();
            var clubDict = allClubs.ToDictionary(c => c.Id, c => c.Name);
            
            m_logger.LogInformation("Number of matches: {Count}", matches.Count);

            // Simulate each match in parallel threads (no database writes)
            // The engine will only read from the database and write results to external storage (e.g., Cloudflare)
            var matchTasks = matches.Select(match => SimulateMatchAsync(match, clubDict)).ToList();
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

            update_leagues_standings();
            // update_club_statistics();
            // update_robot_statistics();

            m_logger.LogInformation("League matches for round {Round} completed", current_round);
        }

        // Todo: We could have a match input class here that only contains the necessary info for the engine to simulate the match,
        // and a match result class that contains the results from the engine.
        // This way we can decouple the database entities from the engine input/output and  avoid accidental database writes in the simulation step.
        private async Task<Match> SimulateMatchAsync(Match match, Dictionary<Guid, string> clubDict)
        {
            // Simulate match in parallel thread - NO database writes here
            m_logger.LogInformation("Simulating match ID: {MatchID}", match.Id);
            
            // TODO: Call engine to simulate match and store results in external storage
            // Return match object with updated scores (but don't save to database)
            // Use pre-loaded club data to avoid database queries during parallel execution
            var home_club_name = clubDict.TryGetValue(match.TeamHomeId, out var homeClubName) ? homeClubName : "Unknown Home Club";
            var away_club_name = clubDict.TryGetValue(match.TeamAwayId, out var awayClubName) ? awayClubName : "Unknown Away Club";
            m_logger.LogInformation("Home club: {HomeClub}, Away club: {AwayClub}", home_club_name, away_club_name);
            var homeTeam = new MatchEngineTeam { Name = home_club_name };
            var awayTeam = new MatchEngineTeam { Name = away_club_name };

            var match_engine = new SimpleMatchEngine();
            match_engine.SetMatchInput(homeTeam, awayTeam);
            match_engine.SetMatchRules(90, false, false);
            var result =match_engine.SimulateMatch();

            match.ScoreHome = result.HomeScore;
            match.ScoreAway = result.AwayScore;
                        
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

        private void update_leagues_standings()
        {
            // logic to update league standings
            int round = m_db.Games.First().CurrentLeagueRound;
            m_logger.LogInformation("Updating league standings for round: {Round}", round);

            var league_system = new LeagueStandings(m_db, m_loggerFactory);
            league_system.UpdateRankings(round);
            m_db.SaveChanges();
        }
    }
}