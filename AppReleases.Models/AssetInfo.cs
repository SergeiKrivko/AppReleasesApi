using System.Text.Json.Serialization;

namespace AppReleases.Models;

public class AssetInfo
{
    [JsonPropertyName("fileName")] public required string FileName { get; set; }
    [JsonPropertyName("fileHash")] public required string FileHash { get; set; }
}