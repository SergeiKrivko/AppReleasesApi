namespace AppReleases.DataAccess.Entities;

public class AssetEntity
{
    public required Guid AssetId { get; init; }
    public required Guid ReleaseId { get; init; }
    public required string FileName { get; init; }
    public required string FileHash { get; init; }
    public required Guid FileId { get; init; }
    public required DateTime CreatedAt { get; init; }

    public virtual ReleaseEntity Release { get; init; } = null!;
}