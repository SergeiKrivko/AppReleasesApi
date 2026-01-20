using System.Text.Json.Serialization;

namespace AppReleases.Models;

public class AssetInfo
{
    [JsonPropertyName("fileName")] public required string FileName { get; init; }
    [JsonPropertyName("fileHash")] public required string FileHash { get; init; }
    [JsonPropertyName("isExecutable")] public bool IsExecutable { get; init; }
}