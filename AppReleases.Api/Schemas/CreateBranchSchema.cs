using System.Text.Json.Serialization;
using AppReleases.Models.Json;

namespace AppReleases.Api.Schemas;

public class CreateBranchSchema
{
    public required string Name { get; set; }
    [JsonConverter(typeof(TimeSpanConverter))]
    public TimeSpan? Duration { get; set; }
    public bool UseDefaultDuration { get; set; } = true;
}