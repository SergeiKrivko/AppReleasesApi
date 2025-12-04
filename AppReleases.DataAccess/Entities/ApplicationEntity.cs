using System.ComponentModel.DataAnnotations;
using AppReleases.Models;

namespace AppReleases.DataAccess.Entities;

public class ApplicationEntity
{
    public required Guid ApplicationId { get; init; }
    [MaxLength(32)] public required string Key { get; init; }
    [MaxLength(100)] public required string Name { get; init; }
    [MaxLength(10000)] public string? Description { get; init; }
    [MaxLength(100)] public required string MainBranch { get; init; }
    public TimeSpan? DefaultReleaseLifetime { get; init; }
    public TimeSpan? DefaultLatestReleaseLifetime { get; init; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? DeletedAt { get; init; }
    public virtual ICollection<BranchEntity> Branches { get; init; } = [];
    public virtual ICollection<InstallerBuilderUsageEntity> InstallerBuilders { get; init; } = [];
}