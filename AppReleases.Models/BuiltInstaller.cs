namespace AppReleases.Models;

public class BuiltInstaller
{
    public required Stream FileStream { get; init; }
    public required string FileName { get; init; }
}

public class BuiltInstallerModel
{
    public Guid FileId { get; init; }
    public required string FileName { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? DownloadedAt { get; init; }
    public DateTime? DeletedAt { get; init; }
}