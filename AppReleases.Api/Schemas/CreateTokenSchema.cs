using System.Text.Json.Serialization;
using AppReleases.Core.Json;

namespace AppReleases.Api.Schemas;

public class CreateTokenSchema
{
    public required string Name { get; init; }
    public required string Mask { get; init; }
    [JsonConverter(typeof(TimeSpanConverter))]
    public DateTime? ExpiresAt { get; init; }
}