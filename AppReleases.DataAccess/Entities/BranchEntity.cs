using System.ComponentModel.DataAnnotations;

namespace AppReleases.DataAccess.Entities;

public class BranchEntity
{
    public Guid BranchId { get; init; }
    public Guid ApplicationId { get; init; }
    [MaxLength(32)] public required string Name { get; init; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? DeletedAt { get; init; }
    public TimeSpan? Duration { get; init; }

    public virtual ICollection<ReleaseEntity> Releases { get; init; } = [];
    public virtual ApplicationEntity Application { get; init; } = null!;
}