using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using AppReleases.Models;

namespace AppReleases.DataAccess.Entities;

public class BuiltInstallerEntity
{
    public Guid Id { get; set; }
    public Guid ReleaseId { get; set; }
    public Guid BuilderId { get; set; }
    public Guid FileId { get; set; }
    [MaxLength(64)] public required string FileName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DownloadedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public virtual InstallerBuilderUsageEntity Builder { get; set; } = null!;
    public virtual ReleaseEntity Release { get; set; } = null!;
}