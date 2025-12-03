using System.Text.Json.Serialization;
using TimeSpanConverter = AppReleases.Models.Json.TimeSpanConverter;

namespace AppReleases.Api.Schemas;

public class AddInstallerBuilderSchema
{
    public required string Key { get; init; }

    [JsonConverter(typeof(TimeSpanConverter))]
    public TimeSpan InstallerLifetime { get; init; }
}