using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.ComponentModel.DataAnnotations;
using GalaxyFootball.Infrastructure.Database;
using Microsoft.AspNetCore.Identity;
using GalaxyFootball.Domain.Entities;

public class LoginModel
{
    [Required]
    public string Username { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}

[ApiController]
[Route("auth")]
public class LoginController : ControllerBase
{
    private readonly IConfiguration m_configuration;
    private readonly ApplicationDbContext m_db;
    private readonly IWebHostEnvironment m_env;

    public LoginController(IConfiguration configuration, ApplicationDbContext db, IWebHostEnvironment env)
    {
        m_configuration = configuration;
        m_db = db;
        m_env = env;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        string role = "User"; // Default role

        // Check for hardcoded admin first
        if (model.Username == "admin" && model.Password == "bergkamp")
        {
            role = "Admin";
            
            var adminClaims = new[]
            {
                new Claim(ClaimTypes.Name, model.Username),
                new Claim(ClaimTypes.Role, role)
            };

            var adminKey = m_configuration["GALAXY_FOOTBALL_JWT_KEY"];
            Console.WriteLine($"LOGIN JWT KEY: {adminKey}");
            var adminSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(adminKey ?? string.Empty));
            var adminCreds = new SigningCredentials(adminSecurityKey, SecurityAlgorithms.HmacSha256);

            var adminToken = new JwtSecurityToken(
                issuer: "galaxy-football-backend",
                audience: "galaxy-football-clients",
                claims: adminClaims,
                expires: DateTime.Now.AddHours(12),
                signingCredentials: adminCreds);

            var adminTokenString = new JwtSecurityTokenHandler().WriteToken(adminToken);
            Console.WriteLine($"JWT TOKEN: {adminTokenString}");

            return Ok(new { token = adminTokenString });
        }

        // Validate user credentials in database
        var user = m_db.Users.FirstOrDefault(u => u.Username == model.Username);
        if (user == null)
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }

        // Verify password - use PasswordHasher in production, plain text comparison otherwise
        // bool passwordValid;
        // if (m_env.IsProduction())
        // {
        //     var hasher = new PasswordHasher<User>();
        //     var verificationResult = hasher.VerifyHashedPassword(user, user.Password, model.Password);
        //     passwordValid = verificationResult == PasswordVerificationResult.Success;
        // }
        // else
        // {
        //     passwordValid = user.Password == model.Password;
        // }

        bool passwordValid = user.Password == model.Password;

        if (!passwordValid)
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, model.Username),
            new Claim(ClaimTypes.Role, role) // Add role claim
        };

        var key = m_configuration["GALAXY_FOOTBALL_JWT_KEY"];
        Console.WriteLine($"LOGIN JWT KEY: {key}");
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key ?? string.Empty));

        var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "galaxy-football-backend",
            audience: "galaxy-football-clients",
            claims: claims,
            expires: DateTime.Now.AddHours(12),
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        Console.WriteLine($"JWT TOKEN: {tokenString}");

        return Ok(new { token = tokenString });
    }
}