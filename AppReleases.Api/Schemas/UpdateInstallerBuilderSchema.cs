using System.Text.Json.Serialization;
using AppReleases.Models.Json;

namespace AppReleases.Api.Schemas;

public class UpdateInstallerBuilderSchema
{
    public string? Name { get; init; }

    [JsonConverter(typeof(TimeSpanConverter))]
    public TimeSpan InstallerLifetime { get; init; }

    public string[] Platforms { get; init; } = [];
}