using System;

namespace GalaxyFootball.Domain.Entities
{
    /// <summary>
    /// Represents the global state of the game.
    /// There can only be one instance of this in the database.
    /// Defines how the calendar and the league system are created.
    /// </summary>
    public class Game
    {
        public Guid Id { get; set; } // Primary key

        // Indexes for step-by-step processing
        public int Year { get; set; } // Current year/season
        public int Day { get; set; } // Current day index

        // Calendar and league system
        public int DaysBetweenGames { get; set; } // Days in-between games
        public int MaxLeagueRounds { get; set; }
        public int CurrentLeagueRound { get; set; }
        public int MaxCupRounds { get; set; }
        public int CurrentCupRound { get; set; }
        public int NumberOfTeamsInLeague { get; set; }

        // Game state
        public bool IsPaused { get; set; }
        public bool IsProcessing { get; set; }
        public bool IsLocked { get; set; }

        // Version info
        public int DatabaseVersion { get; set; }
        public string GameVersion { get; set; } = string.Empty;

    }
}
