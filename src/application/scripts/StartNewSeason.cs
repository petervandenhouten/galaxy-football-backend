using Microsoft.Extensions.Logging;

using GalaxyFootball.Infrastructure.Database;
using GalaxyFootball.Application.Factories;
using GalaxyFootball.Domain.Entities;
using GalaxyFootball.Domain.Utils;

namespace GalaxyFootball.Application.Scripts
{
    /// <summary>
    /// Script to start a new season. Should be called as part of starting a new game or when a new season begins.
    /// - apply promotions and relegations (of last season)
    /// - add pending new users in leagues
    /// - increase year, reset day
    /// - create calendar
    /// - schedule matches
    /// - reset season scores/results of teams and robots
    /// </summary>
    public class StartNewSeason : BaseScript
    {
        private readonly ILogger<StartNewSeason> m_logger;
        public StartNewSeason(ApplicationDbContext db, ILoggerFactory loggerFactory)
            : base(db, loggerFactory)
        {
            m_logger = loggerFactory.CreateLogger<StartNewSeason>();
        }

        public override async Task<Task> Run()
        {
            // Implement the logic to start a new season
            m_logger.LogInformation("Starting a new season...");

            delete_calendar();
            delete_matches_from_database();
            delete_matches_from_storage(); // delete engine files from cloudflare

            set_next_year_and_reset_day();

            var calendarFactory = new CalendarFactory(m_loggerFactory);            
            var param = GameParameters.GetInfo();
            var nr_of_clubs = m_db.Teams.Count();
            var calendar = calendarFactory.CreateCalender(m_db.Games.First(),param,nr_of_clubs);

            m_db.Calendar.AddRange(calendar);

            schedule_matches_for_all_leagues(); // uses calendar and leagues from database
            reset_league_results();
            reset_all_robots_statistics();

            await m_db.SaveChangesAsync();

            return Task.CompletedTask;
        }

        public override bool CanRun()
        {
            var game = m_db.Games.FirstOrDefault();
            return game != null;
        }

        private void delete_calendar()
        {
            var calendarEntries = m_db.Calendar.ToList();
            m_db.Calendar.RemoveRange(calendarEntries);
            m_db.SaveChanges();
            m_logger.LogInformation("Calender cleared for the new season.");
        }   

        private void set_next_year_and_reset_day()
        {
            var game = m_db.Games.FirstOrDefault();
            if (game != null)
            {
                game.Year += 1; // Move to the next year/season
                game.Day = 1;   // Reset to the first day of the new season
                m_db.SaveChanges();
                m_logger.LogInformation($"New season started: Year {game.Year}, Day {game.Day}");
            }
        }

        void delete_matches_from_database()
        {
            // todo not implemented yet
        }

        private void delete_matches_from_storage()
        {
            // todo not implemented yet
        }

        private void schedule_matches_for_all_leagues()
        {
            var calendar = m_db.Calendar.ToList();

            foreach(var league in m_db.Leagues)
            {
                var match_factory = new LeagueMatchFactory();
                var matches = match_factory.create_matches_for_league(league.Id, calendar);

                // Fill in team and stadium guid
                foreach(var match in matches)
                {
                    match.TeamHomeId = GetTeamIdFromIndexInLeague(league.Id, match.TeamHomeIndex);
                    match.TeamAwayId = GetTeamIdFromIndexInLeague(league.Id, match.TeamAwayIndex);
                    match.Stadium    = GetStadiumFromTeam(match.TeamHomeId);
                }

                m_db.Matches.AddRange(matches);
            }
            m_db.SaveChanges();
        }

        private Guid GetStadiumFromTeam(Guid team_id)
        {
            var club = m_db.ClubTeams.FirstOrDefault(c => c.TeamId == team_id);
            if ( club is not null)
            {
                var stadium = m_db.ClubStadiums.FirstOrDefault( c => c.ClubId == club.ClubId);
                if ( stadium is not null )
                {
                    return stadium.StadiumId;
                }
            }
            return Guid.Empty;
        }
        
        private Guid GetTeamIdFromIndexInLeague(Guid league_id, int index)
        {
            var team_in_league = m_db.TeamCompetitions.FirstOrDefault(d => d.CompetitionId == league_id && d.TeamIndex == index);
            if ( team_in_league is not null) return team_in_league.TeamId;
            return Guid.Empty;
        }

        private void reset_all_robots_statistics()
        {
            foreach(var robot in m_db.Robots)
            {
                reset_robot_season_statistics(robot);
            }
            m_db.SaveChanges();
        }

        private void reset_robot_season_statistics(Robot robot)
        {
            if (robot is not null)
            {
                var stats = m_db.RobotStatistics.FirstOrDefault( s => s.Id == robot.RobotSeasonStatistics );
                if ( stats is not null )
                {
                    RobotStatisticsUtility.Reset(stats);
                }
                m_db.SaveChanges();
            }
        }
        
        private void reset_league_results()
        {
            var old_league_results = m_db.LeagueResults.ToList();
            m_db.LeagueResults.RemoveRange(old_league_results);
            var old_league_to_results = m_db.LeagueLeagueResults.ToList();
            m_db.LeagueLeagueResults.RemoveRange(old_league_to_results);

            foreach(var league in m_db.Leagues)
            {
                var team_entries = m_db.TeamCompetitions.Where( t => t.CompetitionId == league.Id).ToList();

                foreach(var team_entry in team_entries)
                {
                    var league_result = new LeagueResult
                    {
                        Id = Guid.NewGuid(),
                        TeamId = team_entry.TeamId,
                        CompetitionId = league.Id,
                        // All attributes are reset by constructor
                    };
                    m_db.LeagueResults.Add(league_result);

                    // todo: this table is not needed???
                    var league_to_result_reference = new LeagueLeagueResult
                    {
                        LeagueId = league.Id,
                        LeagueResultId = league_result.Id
                    };
                    m_db.LeagueLeagueResults.Add(league_to_result_reference);
                }
            }     
            m_db.SaveChanges();       
        }
    }
}
