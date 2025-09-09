using System.Text.Json.Serialization;
using AppReleases.Models.Json;

namespace AppReleases.Api.Schemas;

public class CreateBranchSchema
{
    public required string Name { get; set; }

    [JsonConverter(typeof(TimeSpanConverter))]
    public TimeSpan? ReleaseLifetime { get; set; }

    [JsonConverter(typeof(TimeSpanConverter))]
    public TimeSpan? LatestReleaseLifetime { get; set; }

    public bool UseDefaultReleaseLifetime { get; set; } = true;
}