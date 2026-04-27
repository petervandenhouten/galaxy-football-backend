using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.ComponentModel.DataAnnotations;

public class LoginModel
{
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
}

[ApiController]
[Route("auth")]
public class LoginController : ControllerBase
{
    private readonly IConfiguration m_configuration;

    public LoginController(IConfiguration configuration)
    {
        m_configuration = configuration;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Validate user credentials here in database
        if (model.Username != "admin" || model.Password != "password")
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, model.Username),
            new Claim(ClaimTypes.Role, "Admin") // Add role claim
        };

        var key = m_configuration["GALAXY_FOOTBALL_JWT_KEY"];
        Console.WriteLine($"LOGIN JWT KEY: {key}");
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

        var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "galaxy-football-backend",
            audience: "galaxy-football-clients",
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        Console.WriteLine($"JWT TOKEN: {tokenString}");

        return Ok(new { token = tokenString });
    }
}