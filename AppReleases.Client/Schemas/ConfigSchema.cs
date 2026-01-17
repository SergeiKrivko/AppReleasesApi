using System.Text.Json.Serialization;

namespace AppReleases.Client.Schemas;

internal class ConfigSchema
{
    public required string ApiUrl { get; set; }
    public required Guid ApplicationId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Guid InstalledReleaseId { get; set; }
}