namespace AppReleases.Core.Models;

public class Release
{
    public required Guid Id { get; init; }
    public required Guid ApplicationId { get; init; }
    public required Version Version { get; init; }
    public required Guid PublisherId { get; init; }
    public required string Platform { get; init; }
    public string? ReleaseNotes { get; init; }
    public required DateTime CreatedAt { get; init; }
    public bool IsPrerelease { get; init; }
    public bool IsObsolete { get; init; } = false;
}