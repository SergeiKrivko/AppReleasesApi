using System.Text.Json.Nodes;

namespace AppReleases.Models;

public class InstallerBuilderUsage
{
    public Guid Id { get; init; }
    public required string BuilderKey { get; init; }
    public string? Name { get; init; }
    public required JsonObject Settings { get; init; }
    public TimeSpan? InstallerLifetime { get; init; }
}