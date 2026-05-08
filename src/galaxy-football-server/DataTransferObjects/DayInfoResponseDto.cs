namespace GalaxyFootball.Server.DataTransferObjects
{
    public sealed record DayInfoResponseDto(
        string ApiVersion,
        DateTime GeneratedAt,
        int Day,
        int Year
    );
}
