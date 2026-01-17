using System.Text.Json.Serialization;
using AppReleases.Client.Schemas;

namespace AppReleases.Client;

[JsonSerializable(typeof(UrlResponseSchema))]
[JsonSerializable(typeof(ApplicationSchema))]
[JsonSerializable(typeof(ReleaseSchema))]
[JsonSerializable(typeof(ConfigSchema))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
internal partial class ReleasesJsonSerializerContext : JsonSerializerContext
{
}