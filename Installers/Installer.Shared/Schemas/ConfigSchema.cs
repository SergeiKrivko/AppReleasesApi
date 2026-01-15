using System.Text.Json.Serialization;

namespace Installer.Shared.Schemas;

public class ConfigSchema
{
    public required string ApiUrl { get; set; }
    public required Guid ApplicationId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Guid ReleaseId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Guid InstalledReleaseId { get; set; }

    public AssetSchema[] Assets { get; set; } = [];
}