namespace AppReleases.Models;

public class Token
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required DateTime IssuedAt { get; init; }
    public required DateTime ExpiresAt { get; init; }
    public DateTime? RevokedAt { get; init; }
    public required string Mask { get; init; }
}