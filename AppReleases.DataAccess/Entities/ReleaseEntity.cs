using System.ComponentModel.DataAnnotations;

namespace AppReleases.DataAccess.Entities;

public class ReleaseEntity
{
    public required Guid ReleaseId { get; init; }
    public required Guid BranchId { get; init; }
    [MaxLength(32)] public required string Version { get; init; }
    [MaxLength(32)] public string? Platform { get; init; }
    [MaxLength(10000)] public string? ReleaseNotes { get; init; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? DeletedAt { get; init; }

    public virtual BranchEntity Branch { get; init; } = null!;
    public virtual ICollection<AssetEntity> Assets { get; init; } = [];
    public virtual ICollection<ReleaseAssetEntity> ReleaseAssets { get; init; } = [];
    public virtual ICollection<InstallerEntity> Installers { get; init; } = [];
}