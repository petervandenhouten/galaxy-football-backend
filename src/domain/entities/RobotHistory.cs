namespace GalaxyFootball.Domain.Entities
{
    /// <summary>
    /// Robot History events
    /// Can be a transfer, a upgrade, a cup or league won
    /// </summary>
    public class RobotHistory
    {
        public Guid Id { get; set; }

        public int Year { get; set; }
        public RobotHistoryEvent RobotHistoryEvent { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public enum RobotHistoryEvent
    {
        Transfer,
        Upgrade,
       
        // todo
    }
}