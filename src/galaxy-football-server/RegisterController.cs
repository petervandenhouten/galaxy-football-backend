using GalaxyFootball.Infrastructure.Database;
using GalaxyFootball.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace GalaxyFootball.Server
{
    [ApiController]
    [Route("api")]
    public class RegisterController : ControllerBase
    {
        private readonly ApplicationDbContext m_db;
        private readonly ILogger<RegisterController> m_logger;
        private readonly IWebHostEnvironment m_env;
        private readonly ScriptRunner m_scriptRunner;

        public RegisterController(  ApplicationDbContext db,
                                    ILogger<RegisterController> logger,
                                    IWebHostEnvironment env,
                                    ScriptRunner scriptRunner)
        {
            m_db = db;
            m_logger = logger;
            m_env = env;
            m_scriptRunner = scriptRunner;
        }

        public class RegisterRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Email, username, and password are required.");
            }
            if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
            {
                return BadRequest("First name and last name are required.");
            }

            if (m_db.Users.Any(u => u.Email == request.Email))
            {
                return Conflict("Email is already registered.");
            }
            if (m_db.Users.Any(u => u.Username == request.Username))
            {
                return Conflict("Username is already taken.");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                Username = request.Username,
                CreatedAt = DateTime.UtcNow
            };
            var hasher = new PasswordHasher<User>();
            user.Password = m_env.IsProduction() ? hasher.HashPassword(user, request.Password) : request.Password;

            var player = new Player
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName
            };

            // Create UserPlayer association
            var userPlayer = new UserPlayer
            {
                UserId = user.Id,
                PlayerId = player.Id
            };

            {
                m_db.Users.Add(user);
                m_db.Players.Add(player);
                m_db.UserPlayers.Add(userPlayer);
            }

            await m_db.SaveChangesAsync();

            m_logger.LogInformation("New user registered: {Username} with name as player: {PlayerName}", request.Username, $"{request.FirstName} {request.LastName}");
            return Ok(new { user.Id, user.Email, user.Username });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("delete-all-users")]
        public async Task<IActionResult> DeleteAllUsers()
        {
            await m_scriptRunner.RunScriptByName("DeleteAllUsers");
            return Ok("All users and all players have been deleted by an admin.");
        }
    }
}
