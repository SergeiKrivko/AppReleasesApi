namespace AppReleases.Core.Models;

public class User
{
    public required Guid Id { get; init; }
    public required string Login { get; init; }
    public required string PasswordHash { get; init; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? DeletedAt { get; init; }
}