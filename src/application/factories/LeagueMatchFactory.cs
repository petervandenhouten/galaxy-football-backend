using GalaxyFootball.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace GalaxyFootball.Application.Factories
{
    /// <summary>
    /// Factory for creating league matches
    /// </summary>
    public class LeagueMatchFactory
    {
        private readonly ILogger<LeagueMatchFactory>? m_logger;
        public LeagueMatchFactory(ILoggerFactory? loggerFactory)
        {
            if ( loggerFactory is not null )
            {
                m_logger = loggerFactory.CreateLogger<LeagueMatchFactory>() ;
            }
        }
        public List<Match> create_matches_for_league(Guid league_id, IEnumerable<Calendar> calendar)
        {
            var matches = new List<Match>();

            var nr_league_rounds = calendar.Count(d => d.DayType == CalendarDayType.LeagueMatch);
            int nr_teams = (nr_league_rounds/2) + 1;
            m_logger?.LogInformation("Creating matches for league {league_id} with {nr_teams} teams and {nr_league_rounds} league rounds", league_id, nr_teams, nr_league_rounds);

            if ( nr_teams != 8 )
            {
                m_logger?.LogWarning("LeagueMatchFactory: Unsupported number of teams {nr_teams} for league {league_id}, only 8 teams are supported. No matches created.", nr_teams, league_id);
                return matches;
            }

            for (int round = 1; round <= nr_league_rounds; round++)
            {
                // get days of league round
                var calendar_day = calendar.FirstOrDefault(d => d.DayType == CalendarDayType.LeagueMatch && d.CompetitionRound == round);
                if ( calendar_day is null)
                {
                    m_logger?.LogError("No calendar entry found for league round {round} in league {league_id}. Cannot create matches for this round.", round, league_id);
                    continue;
                }

                // for every match in the round
                for (int matchNr = 1; matchNr <= (nr_teams/2); matchNr++)
                {
                    var match = new Match
                    {
                        Id = Guid.NewGuid(),
                        Day = calendar_day.DayIndex,
                        ScoreHome = 0,
                        ScoreAway = 0,
                        CompetitionType = CalendarDayType.LeagueMatch,
                        CompetitionId = league_id,
                        CompetitionRound = round
                    };

                    (int leagueIndex1, int leagueIndex2) = GetTeamIndexFromSchedule(nr_teams, round, matchNr);
                    match.TeamHomeIndex = leagueIndex1;
                    match.TeamAwayIndex = leagueIndex2;

                    // m_logger?.LogDebug("Creating match {match_id} for league {league_id}, round {round}, match number {matchNr}: team home index {leagueIndex1} vs team away index {leagueIndex2}", match.Id, league_id, round, matchNr, leagueIndex1, leagueIndex2);

                    // Team Id, Stadium id must be filled in by class with access to database
                    // Weather id must be filled in a few days before the match
                    match.TeamHomeId = Guid.Empty;
                    match.TeamAwayId = Guid.Empty;
                    match.Stadium = Guid.Empty;
                    match.Weather = Guid.Empty;

                    matches.Add(match);
                }
            }
            return matches;
        }


        private (int,int) GetTeamIndexFromSchedule(int nr_teams, int round, int match)
        {
            int[,] schedule8 = new int[7, 8] {
                    { 6,5,1,3,4,2,8,7 }, // 1
                    { 3,7,6,2,5,8,1,4 },
                    { 1,7,6,3,5,2,4,8 },
                    { 6,8,7,4,2,1,3,5 }, // 4
                    { 8,1,6,4,7,5,2,3 },
                    { 6,7,2,8,3,4,5,1 },
                    { 4,5,6,1,8,3,7,2 } // 7
            };

            int team_index_home = -1;
            int team_index_away = -1;

            if ( nr_teams == 8 )
            {
                if (round < 8)
                {
                    team_index_home = schedule8[round - 1, match * 2 - 2];
                    team_index_away = schedule8[round - 1, match * 2 - 1];
                }
                else
                {
                    team_index_home = schedule8[round - 8, match * 2 - 1];
                    team_index_away = schedule8[round - 8, match * 2 - 2];
                }
            }
            else
            {
                // unsupported
            }
            return (team_index_home, team_index_away);
        }
    }
}
