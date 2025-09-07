using System.Text.Json.Serialization;
using AppReleases.Models.Json;

namespace AppReleases.Api.Schemas;

public class UpdateApplicationSchema
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string MainBranch { get; set; }
    [JsonConverter(typeof(TimeSpanConverter))]
    public TimeSpan? DefaultDuration { get; set; }
}