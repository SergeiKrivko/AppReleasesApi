using System.Text.Json.Nodes;
using AppReleases.Models;

namespace AppReleases.Core.Abstractions;

public interface IInstallerBuilder
{
    public string Key { get; }

    public string DisplayName { get; }
    public string? Description { get; }

    public Task<BuiltInstaller> Build(InstallerBuilderContext context, CancellationToken cancellationToken = default);
}

public class InstallerBuilderContext
{
    public required Application Application { get; init; }
    public required Release Release { get; init; }
    public IEnumerable<Asset>? Assets { get; init; }
    public required JsonObject Settings { get; init; }
    public string? ApiUrl { get; init; }
}