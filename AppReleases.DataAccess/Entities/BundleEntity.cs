using System.ComponentModel.DataAnnotations;

namespace AppReleases.DataAccess.Entities;

public class BundleEntity
{
    public required Guid BundleId { get; set; }
    public required Guid ReleaseId { get; set; }
    [MaxLength(64)] public required string FileName { get; set; }
    public required Guid FileId { get; set; }
    public required DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public virtual ReleaseEntity Release { get; set; } = null!;
}