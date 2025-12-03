namespace AppReleases.Models;

public class BuiltInstaller
{
    public required Stream File { get; init; }
    public required string FileName { get; init; }
}