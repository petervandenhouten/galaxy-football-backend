namespace GalaxyFootball.Domain.Entities
{
    /// <summary>
    /// Represents a sponsor in the game.
    /// A sponsor can provide financial support to teams and participate in various events.
    /// </summary>
    public class Sponsor
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public int Budget { get; set; }
    }
}
