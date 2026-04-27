using System;

namespace GalaxyFootball.Domain.Entities
{
    /// <summary>
    /// Represents a computer-controlled player (autocoach) that manages a team.
    /// </summary>
    public class AutoCoach
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        // Enums for AI behavior
        public PreferredLineupPickStrategy PreferredLineupPickStrategy { get; set; }
        public PreferredLineUpFormation PreferredLineUpFormation { get; set; }
        public LineupFormationSelectionMethod LineupFormationSelectionMethod { get; set; }
        public LineupPickStrategySelectionMethod LineupPickStrategySelectionMethod { get; set; }
    }

    // Enum definitions (placeholders, define values as needed)
    public enum PreferredLineupPickStrategy
    {
        Random,
        BestPlayers,
        YouthDevelopment
    }

    public enum PreferredLineUpFormation
    {
        Formation442,
        Formation433,
        Formation352
    }

    public enum LineupFormationSelectionMethod
    {
        Fixed,
        Adaptive
    }

    public enum LineupPickStrategySelectionMethod
    {
        Fixed,
        Rotating
    }

    /// <summary>
    /// Associative entity linking an autocoach to a club and a team.
    /// </summary>
    public class AutoCoachClubTeam
    {
        public Guid AutoCoachId { get; set; }
        public Guid ClubId { get; set; }
        public Guid TeamId { get; set; }
    }
}
