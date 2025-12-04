using System.ComponentModel.DataAnnotations;

namespace AppReleases.DataAccess.Entities;

public class InstallerBuilderUsageEntity
{
    public Guid Id { get; set; }
    public Guid ApplicationId { get; set; }
    [MaxLength(32)] public required string BuilderKey { get; set; }
    [MaxLength(128)] public string? Name { get; set; }
    [MaxLength(4096)] public string? Settings { get; set; }
    public TimeSpan? InstallerLifetime { get; set; }
    public required string[] Platforms { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime DeletedAt { get; set; }

    public virtual ApplicationEntity Application { get; set; } = null!;
    public virtual ICollection<BuiltInstallerEntity> BuiltInstallers { get; set; } = null!;
}