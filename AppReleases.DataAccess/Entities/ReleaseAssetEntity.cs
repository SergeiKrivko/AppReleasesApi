using AppReleases.Core.Models;

namespace AppReleases.DataAccess.Entities;

public class ReleaseAssetEntity
{
    public required Guid Id { get; init; }
    public required Guid ReleaseId { get; init; }
    public required Guid AssetId { get; init; }

    public virtual AssetEntity Asset { get; init; } = null!;
    public virtual ReleaseEntity Release { get; init; } = null!;
}