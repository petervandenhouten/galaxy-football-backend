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
        public Guid CompetitionId { get; set; }

        // Home stats
        public int HomePlayed { get; set; }
        public int HomeWins { get; set; }
        public int HomeDraws { get; set; }
        public int HomeLosses { get; set; }
        public int HomeGoalsFor { get; set; }
        public int HomeGoalsAgainst { get; set; }

        // Away stats
        public int AwayPlayed { get; set; }
        public int AwayWins { get; set; }
        public int AwayDraws { get; set; }
        public int AwayLosses { get; set; }
        public int AwayGoalsFor { get; set; }
        public int AwayGoalsAgainst { get; set; }

        // Streaks
        public int WinningStreak { get; set; }
        public int LosingStreak { get; set; }

        // Match results string (e.g., "WWWLDD")
        public string MatchResults { get; set; } = string.Empty;
    }
}
