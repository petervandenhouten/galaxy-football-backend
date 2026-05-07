namespace GalaxyFootball.Server.DataTransferObjects
{
    public sealed record LeagueStandingsResponseDto(
        string ApiVersion,
        DateTime GeneratedAt,
        LeagueReferenceDto League,
        SeasonInfoDto Season,
        IReadOnlyList<LeagueStandingEntryDto> Standings);

    public sealed record LeagueReferenceDto(Guid Id, int Level, int Number);

    public sealed record SeasonInfoDto(int? Year, int? Day);

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