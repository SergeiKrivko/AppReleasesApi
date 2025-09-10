namespace AppReleases.Models;

public class Installer
{
    public required Guid InstallerId { get; set; }
    public required Guid ReleaseId { get; set; }
    public required string FileName { get; set; }
    public required Guid FileId { get; set; }
    public required DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}