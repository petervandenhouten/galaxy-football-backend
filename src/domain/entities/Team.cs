using System;

namespace GalaxyFootball.Domain.Entities
{
    /// <summary>
    /// Represents a player-controlled or autocoach-controlled team.
    /// The team participates in matches, leagues, and cups, and owns a set of robots for line-ups.
    /// </summary>
    public class Team
    {
        public Guid Id { get; set; }
    }

    /// <summary>
    /// Associative entity linking a robot to a team.
    /// </summary>
    public class TeamRobot
    {
        public Guid TeamId { get; set; }
        public Guid RobotId { get; set; }
    }

    /// <summary>
    /// Associative entity linking a team to a competition.
    /// </summary>
    public class TeamCompetition
    {
        public Guid TeamId { get; set; }
        public Guid CompetitionId { get; set; }
    }

    /// <summary>
    /// Associative entity linking a match line-up to a team.
    /// </summary>
    public class TeamMatchLineup
    {
        public Guid TeamId { get; set; }
        public Guid MatchLineupId { get; set; }
    }

    /// <summary>
    /// Associative entity linking a saved line-up to a team.
    /// </summary>
    public class TeamSavedLineup
    {
        public Guid TeamId { get; set; }
        public Guid SavedLineupId { get; set; }
    }
}
