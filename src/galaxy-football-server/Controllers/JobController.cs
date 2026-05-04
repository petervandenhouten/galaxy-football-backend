using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using GalaxyFootball.Infrastructure.Database;

[ApiController]
[Route("api/job")]
public class JobController : ControllerBase
{
    private readonly ILogger<JobController> m_logger;
    private readonly JobService m_jobService;
    private readonly IConfiguration m_configuration;
    private readonly ApplicationDbContext m_db;

    public JobController(ILogger<JobController> logger, JobService job_service, IConfiguration configuration, ApplicationDbContext db)
    {
        m_logger        = logger;
        m_jobService    = job_service;
        m_configuration = configuration;
        m_db            = db;
    }

    [HttpPost("internal/run-daily-job")]
    public async Task<IActionResult> RunInternal(
        [FromHeader(Name = "X-API-KEY")] string apiKey)
    {
        if (apiKey != m_configuration["GALAXY_FOOTBALL_API_KEY"])
            return Unauthorized();

        m_logger.LogInformation("RunInternal called: code=200 (job started)");
        await m_jobService.RunIfNeeded();
        return Ok();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("admin/run-daily-job")]
    public async Task<IActionResult> RunAdmin()
    {
        await m_jobService.ForceRun();

        var game = await m_db.Games.AsNoTracking().FirstOrDefaultAsync();
        if (game == null)
            return Ok(new { message = "Daily job completed." });

        return Ok(new { message = $"Daily job completed for day {game.Day-1}" });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("admin/start-new-game")]
    public async Task<IActionResult> StartNewGame()
    {
        await m_jobService.ForceStartNewGame();

        var userCount = await m_db.Users.AsNoTracking().CountAsync();
        var leagueCount = await m_db.Leagues.AsNoTracking().CountAsync();

        return Ok(new { message = $"New game started with {userCount} users, {leagueCount} leagues were created." });
    }

}
