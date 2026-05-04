namespace GalaxyFootball.Domain.Entities
{
    public class Match
    {
        /// <summary>
        /// File with Database key ID also references engine output match file in storage
        /// </summary>
        public Guid Id { get; set; }
        public int Day {get; set;}
        public int Year {get; set;}
        public int TeamHomeIndex { get; set; }
        public int TeamAwayIndex { get; set; }
        public Guid TeamHomeId { get; set; }
        public Guid TeamAwayId { get; set; }
        public int ScoreHome { get; set; }
        public int ScoreAway { get; set; }
        public CalendarDayType CompetitionType { get; set; } // defined by from Calender, CUP/LEAGUE
        public Guid CompetitionId { get; set; } // e.g. LeagueID
        public int CompetitionRound { get; set; } // One-based index of round in a competition
        public Guid Stadium { get; set; }
        public Guid Weather { get; set; }
    }

    public class WeatherConditions
    {
        public Guid Id { get; set; }
        public int Temperature { get; set;}        
        public int Rain { get; set;}        
        public int Wind { get; set;} // 0..100 > -50...+50
    }
}
