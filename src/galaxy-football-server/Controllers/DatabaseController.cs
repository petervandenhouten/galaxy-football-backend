using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using GalaxyFootball.Infrastructure.Database;

[ApiController]
[Route("api")]
public class DatabaseController : ControllerBase
{
    private readonly ILogger<DatabaseController> m_logger;
    private readonly ApplicationDbContext m_db;

    public DatabaseController(ApplicationDbContext db, ILogger<DatabaseController> logger)
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
}