namespace GalaxyFootball.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }          // Primary key

        public string Username { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!; // hashed password

        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Associative entity linking a user to a player.
    /// </summary>
    public class UserPlayer
    {
        public Guid UserId { get; set; }
        public Guid PlayerId { get; set; }
    }
}