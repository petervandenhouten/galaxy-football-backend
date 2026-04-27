namespace GalaxyFootball.Domain.Entities
{
    /// <summary>
    /// Represents a person controlling a team as a coach.
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Player's first name.
        /// </summary>
        public string FirstName { get; set; } = null!;

        /// <summary>
        /// Player's last name.
        /// </summary>
        public string LastName { get; set; } = null!;

    }

    /// <summary>
    /// Associative entity linking a player to a club and a team.
    /// </summary>
    public class PlayerClubTeam
    {
        public Guid PlayerId { get; set; }
        public Guid ClubId { get; set; }
        public Guid TeamId { get; set; }
    }
}
