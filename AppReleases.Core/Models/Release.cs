namespace AppReleases.Core.Models;

public class Release
{
    public required Guid Id { get; init; }
    public required Guid BranchId { get; init; }
    public required Version Version { get; init; }
    public required string Platform { get; init; }
    public string? ReleaseNotes { get; init; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? DeletedAt { get; init; }
}