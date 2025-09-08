namespace AppReleases.Models;

public class Release
{
    public required Guid Id { get; init; }
    public required Guid BranchId { get; init; }
    public required Version Version { get; init; }
    public string? Platform { get; init; }
    public string? ReleaseNotes { get; set; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? DeletedAt { get; init; }
}