using System;

namespace GalaxyFootball.Domain.Entities
{
    /// <summary>
    /// Represents the competition standing/ranking of a team in a league for the current season.
    /// Associates teams and competitions.
    /// </summary>
    public class LeagueResult
    {
        public Guid Id { get; set; }
        public Guid TeamId { get; set; }
        public Guid CompetitionId { get; set; } // League Id
        public int Ranking { get; set; } = 0;
        public int PreviousRanking { get; set; } = 0;
        public int Points { get; set; } = 0;

        // Home stats
        public int HomePlayed { get; set; } = 0;
        public int HomeWins { get; set; } = 0;
        public int HomeDraws { get; set; } = 0;
        public int HomeLosses { get; set; } = 0;
        public int HomeGoalsFor { get; set; } = 0;
        public int HomeGoalsAgainst { get; set; } = 0;

        // Away stats
        public int AwayPlayed { get; set; } = 0;
        public int AwayWins { get; set; } = 0;
        public int AwayDraws { get; set; } = 0;
        public int AwayLosses { get; set; } = 0;
        public int AwayGoalsFor { get; set; } = 0;
        public int AwayGoalsAgainst { get; set; } = 0;

        // Streaks
        public int WinningStreak { get; set; } = 0;
        public int LosingStreak { get; set; } = 0;

        // Match results string (e.g., "WWWLDD")
        public string MatchResults { get; set; } = string.Empty;
    }
}
