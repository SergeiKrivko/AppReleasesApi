using System.ComponentModel.DataAnnotations;

namespace AppReleases.DataAccess.Entities;

public class InstallerBuilderUsageEntity
{
    public Guid Id { get; set; }
    public Guid ApplicationId { get; set; }
    [MaxLength(32)] public required string BuilderKey { get; set; }
    [MaxLength(1024)] public string? Settings { get; set; }

    public ApplicationEntity Application { get; set; } = null!;
}