using System;

namespace GalaxyFootball.Domain.Entities
{
    /// <summary>
    /// Represents the global state of the game.
    /// There can only be one instance of this in the database.
    /// </summary>
    public class Game
    {
        public Guid Id { get; set; }          // Primary key
        
        /// <summary>
        /// Indicates if the game is currently paused.
        /// </summary>
        public bool IsPaused { get; set; }

        /// <summary>
        /// Indicates if the game is locked for new input.
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        /// The current date of the game.
        /// </summary>
        public DateTime GameDate { get; set; }

        /// <summary>
        /// Indicates that batch processing is actively running
        /// </summary>
        public bool IsBatchProcessing { get; set; }

        /// <summary>
        /// The database version for migration tracking
        /// </summary>
        public int DatabaseVersion { get; set; }

    }
}
