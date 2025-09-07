using System.ComponentModel.DataAnnotations;

namespace AppReleases.DataAccess.Entities;

public class AssetEntity
{
    public required Guid AssetId { get; init; }
    [MaxLength(100)] public required string FileName { get; init; }
    [MaxLength(100)] public required string FileHash { get; init; }
    public required Guid FileId { get; init; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? DeletedAt { get; init; }

    public virtual ICollection<ReleaseEntity> Releases { get; init; } = [];
    public virtual ICollection<ReleaseAssetEntity> ReleaseAssets { get; init; } = [];
}