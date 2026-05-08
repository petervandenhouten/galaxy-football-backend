namespace GalaxyFootball.Server.DataTransferObjects
{
    public sealed record LeagueStandingsResponseDto(
        string ApiVersion,
        DateTime GeneratedAt,
        LeagueReferenceDto League,
        IReadOnlyList<LeagueStandingEntryDto> Standings);

    public sealed record LeagueReferenceDto(Guid Id, int Level, int Number);

    // Removed SeasonInfoDto (year and day now come from another endpoint)

    public sealed record LeagueStandingEntryDto(
        int Rank,
        int PreviousRank,
        Guid TeamId,
        string TeamName,
        int Points,
        int Played,
        int Wins,
        int Draws,
        int Losses,
        int GoalsFor,
        int GoalsAgainst,
        int GoalDifference,
        IReadOnlyList<string> Form);
}