namespace AppReleases.Core.Models;

public class Branch
{
    public Guid Id { get; init; }
    public Guid ApplicationId { get; init; }
    public required string Name { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? DeletedAt { get; init; }
    public bool IsMerged { get; init; }
}