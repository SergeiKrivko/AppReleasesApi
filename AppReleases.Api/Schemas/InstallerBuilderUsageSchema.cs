using System.Text.Json.Nodes;

namespace AppReleases.Api.Schemas;

public class InstallerBuilderUsageSchema
{
    public Guid Id { get; init; }
    public required string BuilderKey { get; init; }
    public string? Name { get; init; }
    public required object Settings { get; init; }
    public string[] Platforms { get; init; } = [];
    public TimeSpan? InstallerLifetime { get; init; }
}