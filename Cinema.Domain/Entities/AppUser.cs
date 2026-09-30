namespace Cinema.Domain.Entities;

public class AppUser
{
    public int UserId { get; set; }
    public string Login { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string? FullName { get; set; }
    public string Role { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}