using System.ComponentModel.DataAnnotations;

namespace AppReleases.DataAccess.Entities;

public class ApplicationEntity
{
    public required Guid ApplicationId { get; init; }
    [MaxLength(32)] public required string Key { get; init; }
    [MaxLength(100)] public string? Name { get; init; }
    [MaxLength(10000)] public string? Description { get; init; }
    public required Guid UserId { get; init; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? DeletedAt { get; init; }

    public virtual UserEntity User { get; init; } = null!;
    public virtual ICollection<ReleaseEntity> Releases { get; init; } = [];
    public virtual ICollection<BranchEntity> Branches { get; init; } = [];
    public virtual ICollection<TokenEntity> Tokens { get; init; } = [];
}