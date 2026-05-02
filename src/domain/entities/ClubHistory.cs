namespace GalaxyFootball.Domain.Entities
{

    /// <summary>
    /// Club History events
    /// Can be a transfer, a new sponsor, a cup or league won, promotion, relagation
    /// </summary>
    public class ClubHistory
    {
        public Guid Id { get; set; }
        public int Year { get; set; }
        public ClubHistoryEvent ClubHistoryEvent { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public enum ClubHistoryEvent
    {
        NewSponsor,
        NewCoach,
        
        // todo
    }

}