using GalaxyFootball.Infrastructure.Database;
using GalaxyFootball.Server.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/leagues")]
public class LeagueController : ControllerBase
{
    private readonly ApplicationDbContext m_db;

    public LeagueController(ApplicationDbContext db)
    {
        m_db = db;
    }

    [HttpGet("level/{level:int}/number/{number:int}/standings")]
    [ProducesResponseType(typeof(LeagueStandingsResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStandings(int level, int number)
    {
        var league = await m_db.Leagues
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Level == level && item.Number == number);

        if (league == null)
        {
            return NotFound(new { error = $"League level {level} number {number} was not found." });
        }

        var game = await m_db.Games
            .AsNoTracking()
            .FirstOrDefaultAsync();

        var standings = await (
            from result in m_db.LeagueResults.AsNoTracking()
            join clubTeam in m_db.ClubTeams.AsNoTracking() on result.TeamId equals clubTeam.TeamId into clubTeamJoin
            from clubTeam in clubTeamJoin.DefaultIfEmpty()
            join club in m_db.Clubs.AsNoTracking() on clubTeam.ClubId equals club.Id into clubJoin
            from club in clubJoin.DefaultIfEmpty()
            where result.CompetitionId == league.Id
            orderby result.Ranking, result.TeamId
            select new LeagueStandingEntryDto(
                result.Ranking,
                result.PreviousRanking,
                result.TeamId,
                club != null ? club.Name : result.TeamId.ToString(),
                result.Points,
                result.HomePlayed + result.AwayPlayed,
                result.HomeWins + result.AwayWins,
                result.HomeDraws + result.AwayDraws,
                result.HomeLosses + result.AwayLosses,
                result.HomeGoalsFor + result.AwayGoalsFor,
                result.HomeGoalsAgainst + result.AwayGoalsAgainst,
                (result.HomeGoalsFor + result.AwayGoalsFor) - (result.HomeGoalsAgainst + result.AwayGoalsAgainst),
                ToForm(result.MatchResults))
        ).ToListAsync();

        return Ok(new LeagueStandingsResponseDto(
            "1",
            DateTime.UtcNow,
            new LeagueReferenceDto(league.Id, league.Level, league.Number),
            new SeasonInfoDto(game?.Year + 2049, game?.Day),
            standings));
    }

    private static IReadOnlyList<string> ToForm(string matchResults)
    {
        if (string.IsNullOrWhiteSpace(matchResults))
        {
            return Array.Empty<string>();
        }

        return matchResults
            .Take(5)
            .Select(character => character.ToString())
            .ToArray();
    }

}