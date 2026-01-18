using System.Text.Json.Serialization;

namespace AppReleases.Installers.Shared;

internal class ConfigSchema
{
    public required string ApiUrl { get; set; }
    public required Guid ApplicationId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Guid ReleaseId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Guid InstalledReleaseId { get; set; }

    public InstalledAssetSchema[] Assets { get; set; } = [];

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? InstallationPath { get; set; }
}

internal class InstalledAssetSchema
{
    public required string FileName { get; set; }
    public string? FileHash { get; set; }
    public required string InstalledFileName { get; set; }
}