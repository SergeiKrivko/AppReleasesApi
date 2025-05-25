using System.ComponentModel.DataAnnotations;

namespace AppReleases.DataAccess.Entities;

public class ReleaseEntity
{
    public required Guid ReleaseId { get; init; }
    public required Guid ApplicationId { get; init; }
    public required Version Version { get; init; }
    [MaxLength(32)] public required string Platform { get; init; }
    [MaxLength(10000)] public string? ReleaseNotes { get; init; }
    public required DateTime CreatedAt { get; init; }
    public bool IsPrerelease { get; init; } = false;
    public bool IsObsolete { get; init; } = false;
    public Guid? BranchId { get; init; }

    public virtual ApplicationEntity Application { get; init; } = null!;
    public virtual BranchEntity? Branch { get; init; }
    public virtual ICollection<AssetEntity> Assets { get; init; } = [];
}