using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using GalaxyFootball.Infrastructure.Database;

[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly ILogger<GameController> m_logger;
    private readonly ApplicationDbContext m_db;

    public GameController(ApplicationDbContext db, ILogger<GameController> logger)
    {
        m_db = db;
        m_logger = logger;
    }

    [HttpGet("database-locked")]
    public async Task<IActionResult> CheckIfDatabaseIsLocked()
    {
        // Assumes only one Game row exists (singleton pattern)
        var game = await m_db.Games.FirstOrDefaultAsync();
        if (game == null)
        {
            return NotFound(new { error = "No game state found." });
        }
        return Ok(new { isLocked = game.IsLocked, isProcessing = game.IsProcessing, isPaused = game.IsPaused });
    }

    [HttpGet("day-info")]
    public async Task<IActionResult> GetDayInfo()
    {
        // Assumes only one Game row exists (singleton pattern)
        var game = await m_db.Games.FirstOrDefaultAsync();
        if (game == null)
        {
            return NotFound(new { error = "No game state found." });
        }
        return Ok(new { day = game.Day, year= game.Year+2049 });
    }

}