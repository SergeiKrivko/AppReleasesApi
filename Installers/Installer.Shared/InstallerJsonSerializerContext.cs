using System.Text.Json.Serialization;
using Installer.Shared.Schemas;

namespace Installer.Shared;

[JsonSerializable(typeof(UrlResponseSchema))]
[JsonSerializable(typeof(AssetsPackSchema))]
[JsonSerializable(typeof(ApplicationSchema))]
[JsonSerializable(typeof(ReleaseSchema))]
[JsonSerializable(typeof(ConfigSchema))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
internal partial class InstallerJsonSerializerContext : JsonSerializerContext
{
}