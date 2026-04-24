namespace GalaxyFootball.Domain.Entities
{
public class User
{
    public Guid Id { get; set; }          // Primary key

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!; // hashed password

    public DateTime CreatedAt { get; set; }
}
}