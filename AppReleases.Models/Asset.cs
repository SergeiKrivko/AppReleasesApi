namespace AppReleases.Models;

public class Asset
{
    public required Guid Id { get; init; }
    public required string FileName { get; init; }
    public required string FileHash { get; init; }
    public required Guid FileId { get; init; }
    public required DateTime CreatedAt { get; init; }
}