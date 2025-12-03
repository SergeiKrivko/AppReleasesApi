using System.Text.Json.Nodes;

namespace AppReleases.Models;

public class InstallerBuilderUsage
{
    public required string BuilderKey { get; init; }
    public required JsonObject Settings { get; init; }
}