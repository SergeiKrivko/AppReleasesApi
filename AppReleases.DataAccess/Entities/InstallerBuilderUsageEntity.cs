using System.ComponentModel.DataAnnotations;

namespace AppReleases.DataAccess.Entities;

public class InstallerBuilderUsageEntity
{
    public Guid Id { get; set; }
    public Guid ApplicationId { get; set; }
    [MaxLength(32)] public required string BuilderKey { get; set; }
    [MaxLength(1024)] public string? Settings { get; set; }
    public TimeSpan? InstallerLifetime { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime DeletedAt { get; set; }

    public virtual ApplicationEntity Application { get; set; } = null!;
    public virtual ICollection<BuiltInstallerEntity> BuiltInstallers { get; set; } = null!;
}