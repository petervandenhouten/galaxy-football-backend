using System;
using System.Collections.Generic;

namespace GalaxyFootball.Domain.Entities
{
    /// <summary>
    /// Represents the organization of a football club 
    /// A club defines identity, economy, and long-term progression. It owns exactly one team.
    /// In other tables the club is linked to a player, team, stadium or sponsor
    /// </summary>
    public class Club
    {
        // Core
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string PlanetName { get; set; } = null!;
        public int FoundationYear { get; set; }

        // Identity
        public string PrimaryColor { get; set; } = null!;
        public string SecondaryColor { get; set; } = null!;
        public string TertiaryColor { get; set; } = null!;
        public string UniformStyle { get; set; } = null!;
        public string LogoReference { get; set; } = null!;

        // Economy
        public int Balance { get; set; }
        public int LastIncome { get; set; }
        public int LastExpenses { get; set; }

        // Fans
        public int FanCount { get; set; }
        public int FanHappiness { get; set; }
        public int FanExpectations { get; set; }

        // Reputation
        public int ClubRankingPoints { get; set; }
        public int SeasonGamesWon { get; set; }
        public int SeasonGamesLost { get; set; }
        public int SeasonGamesDrawn { get; set; }
        public int SeasonGoalsScored { get; set; }
        public int SeasonGoalsConceded { get; set; }
        public int WinningStreak { get; set; } // Cup and league
        public int LosingStreak { get; set; } // Cup and league

        // History
        public List<SeasonResult> LeagueResults { get; set; } = new();
        public List<CupResult> CupResults { get; set; } = new();
        public int AllTimeGamesWon { get; set; }
        public int AllTimeGamesLost { get; set; }
        public int AllTimeGamesDrawn { get; set; }
        public int AllTimeGoalsScored { get; set; }
        public int AllTimeGoalsConceded { get; set; }
    }

    /// <summary>
    /// Represents a club's result in a season (league or cup)
    /// </summary>
    public class SeasonResult
    {
        public int SeasonYear { get; set; }
        public int DivisionLevel { get; set; } 
        public int DivisionNumber { get; set; } 
        public int Ranking { get; set; } 
        public string Result { get; set; } = null!;// points, games won/lost/drawn, goals scored/conceded, etc.
    }

/// <summary>
    /// Represents a club's result in a cup competition
    /// </summary>
    public class CupResult
    {
        public int SeasonYear { get; set; }
        public int Ranking { get; set; } // e.g., 1 for champion, 2 for runner-up, 3 for semifinalist, etc.
        public string Result { get; set; } = null!;// e.g., "Lost 1-2 to X in round Y"
    }

        /// <summary>
    /// Associative entity linking a club to a team.
    /// </summary>
    public class ClubTeam
    {
        public Guid ClubId { get; set; }
        public Guid TeamId { get; set; }
    }

    /// <summary>
    /// Associative entity linking a club to a stadium.
    /// </summary>
    public class ClubStadium
    {
        public Guid ClubId { get; set; }
        public Guid StadiumId { get; set; }
    }

    /// <summary>
    /// Associative entity linking a club to a sponsor.
    /// </summary>
    public class ClubSponsor
    {
        public Guid ClubId { get; set; }
        public Guid SponsorId { get; set; }
    }

}
