namespace GalaxyFootball.Domain.Entities
{
    public class Calendar
    {
        // Day index in the calendar (e.g., 1, 2, 3...)
        // Used as database key
        public int DayIndex { get; set; }

        // What happens on this day
        public CalendarDayType DayType { get; set; }

        // When Cup or League match
        public int? CompetitionId { get; set; }
        public int? CompetitionRound { get; set; }

        // Optional script to run
        public string? ScriptToRun { get; set; }
    }

    public enum CalendarDayType
    {
        Preseason = 0,
        DraftEvent = 1,
        CupMatch = 2,
        LeagueMatch = 3,
        FastestPlayerEvent = 4,
        PenaltyCupEvent = 5,
        FriendlyMatch = 6,
        AfterSeason = 7,
        Idle = 8
    }
}