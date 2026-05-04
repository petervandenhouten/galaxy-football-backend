public class GameParameters
{
    public int DaysBetweenMatches { get; set; }
    public int PreSeasonDays { get; set; }
    public int AfterSeasonDays { get; set; }
    public int FriendlyMatches { get; set; }
    public int NumberOfTeamsInLeague { get; set; }
    public string Version { get; set; } = string.Empty;
    public int DatabaseVersion { get; set; }

    public static GameParameters GetInfo()
    {
        return new GameParameters
        {
            Version                 = "0.0.2",
            DaysBetweenMatches      = 2,
            PreSeasonDays           = 7,
            AfterSeasonDays         = 4,
            FriendlyMatches         = 3,
            NumberOfTeamsInLeague   = 8,
            DatabaseVersion         = 10
        };
    }
}