using Microsoft.Extensions.Logging;
using GalaxyFootball.Infrastructure.Database;
using GalaxyFootball.Domain.Entities;

public class LeagueStandings
{
    private readonly ILogger<LeagueStandings> m_logger;
    private readonly ApplicationDbContext m_db;
    public LeagueStandings(ApplicationDbContext db, ILoggerFactory loggerFactory)
    {
        m_db = db;
        m_logger = loggerFactory.CreateLogger<LeagueStandings>();
    }

    public void UpdateRankings(int round)
    {
        // Get all matches for the current round
        var matches = m_db.Matches.Where(m => m.CompetitionRound == round && m.CompetitionType == CalendarDayType.LeagueMatch).ToList();

        m_logger.LogInformation("Updating league standings for round: {Round}. Number of matches: {MatchCount}", round, matches.Count);

        // Process each match result and update standings
        foreach (var match in matches)
        {
            if ( round == 1 )
            m_logger.LogInformation("Processing match ID: {MatchID} between Team {HomeTeamID} and Team {AwayTeamID}", match.Id, match.TeamHomeId, match.TeamAwayId);

            update_standing_with_match_result(match.CompetitionId,match.TeamHomeId, match.ScoreHome, match.ScoreAway, true);
            update_standing_with_match_result(match.CompetitionId, match.TeamAwayId, match.ScoreAway, match.ScoreHome, false);
        }

        var leagues = m_db.Leagues.ToList();
        foreach (var league in leagues)
        {
            var league_results = m_db.LeagueResults.Where(lr => lr.CompetitionId == league.Id).ToList();
            var ranked_results = league_results.OrderByDescending(lr => lr.Points)
                                              .ThenByDescending(lr => lr.HomeGoalsFor + lr.AwayGoalsFor) // Goal difference
                                              .ThenByDescending(lr => lr.HomeGoalsFor) // Goals scored
                                              .ToList();

            for (int i = 0; i < ranked_results.Count; i++)
            {
                ranked_results[i].PreviousRanking = ranked_results[i].Ranking;
                ranked_results[i].Ranking = i + 1;
                m_logger.LogInformation("Updated ranking for Team ID: {TeamID} in League ID: {LeagueID}. New Ranking: {Ranking}", ranked_results[i].TeamId, league.Id, ranked_results[i].Ranking);
            }
        }
    }

    private void update_standing_with_match_result(Guid leagueId, Guid teamId, int goals_for, int goals_against, bool is_home_team)
    {
        var league_result = m_db.LeagueResults.FirstOrDefault(lr => lr.TeamId == teamId && lr.CompetitionId == leagueId);
        if (league_result is not null)
        {
            int win  = (is_home_team && goals_for > goals_against) || (!is_home_team && goals_for < goals_against) ? 1 : 0;
            int draw = goals_for == goals_against ? 1 : 0;
            int loss = (is_home_team && goals_for < goals_against) || (!is_home_team && goals_for > goals_against) ? 1 : 0;

            if ( is_home_team )
            {
                league_result.HomePlayed       += 1;
                league_result.HomeGoalsFor     += goals_for;
                league_result.HomeGoalsAgainst += goals_against;
                league_result.HomeWins         += win;
                league_result.HomeDraws        += draw;
                league_result.HomeLosses       += loss;
            }
            else
            {
                league_result.AwayPlayed       += 1;
                league_result.AwayGoalsFor     += goals_for;
                league_result.AwayGoalsAgainst += goals_against;
                league_result.AwayWins         += win;
                league_result.AwayDraws        += draw;
                league_result.AwayLosses       += loss;
            }

            if (win == 1)
            {
                league_result.Points += 3; // Win
            }
            else if (draw == 1)
            {
                league_result.Points += 1; // Draw
            }

            if ( win == 1)
            {
                league_result.WinningStreak += 1;
                league_result.LosingStreak = 0;
            }
            else if ( loss == 1 )
            {
                league_result.LosingStreak += 1;
                league_result.WinningStreak = 0;
            }
            else
            {
                // Draw resets both streaks
                league_result.WinningStreak = 0;
                league_result.LosingStreak = 0;
            }

            var new_match_result = win == 1 ? "W" : draw == 1 ? "D" : "L";
            league_result.MatchResults = new_match_result + league_result.MatchResults;

            m_logger.LogInformation("Updated league result for Team ID: {TeamID}. Points: {Points}", teamId, league_result.Points);
        }
        else
        {
            m_logger.LogWarning("No existing league result found for Team ID: {TeamID} in Competition ID: {CompetitionID}", teamId, leagueId);
        }
    }
}