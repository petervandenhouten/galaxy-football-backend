using System;

namespace GalaxyFootball.Domain.Entities
{
    /// <summary>
    /// Represents a stadium with its own independent attributes.
    /// </summary>
    public class Stadium
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public string PitchType { get; set; } = string.Empty;
        public string Planet { get; set; } = string.Empty;
        public string WeatherTendencies { get; set; } = string.Empty;
        public string Facilities { get; set; } = string.Empty;
        public int TicketPrice { get; set; }
        public int MaintenanceCost { get; set; }
    }
}
