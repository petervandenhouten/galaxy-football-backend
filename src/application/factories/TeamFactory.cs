using System;
using GalaxyFootball.Domain.Entities;

namespace GalaxyFootball.Application.Factories
{
    /// <summary>
    /// Factory for creating Team objects and assigning them to a club by club ID.
    /// </summary>
    public class TeamFactory
    {
        /// <summary>
        /// Instantiates a Team object and assigns it to a club by club ID.
        /// </summary>
        /// <param name="clubId">The ID of the club to assign the team to.</param>
        /// <returns>The created Team object.</returns>
        static public Team CreateTeamForClub(Guid clubId)
        {
            // The Team class does not have a ClubId property, so this is a placeholder for assignment logic.
            // If you add a ClubId property to Team, set it here.
            var team = new Team { Id = Guid.NewGuid() };
            // Example: team.ClubId = clubId;
            return team;
        }
    }
}
