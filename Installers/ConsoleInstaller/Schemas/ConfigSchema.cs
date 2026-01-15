namespace ConsoleInstaller.Schemas;

public class ConfigSchema
{
    public required string ApiUrl { get; set; }
    public required Guid ApplicationId { get; set; }
    public required Guid InstalledReleaseId { get; set; }
    public ConfigAssetSchema[] Assets { get; set; } = [];
}

public class ConfigAssetSchema
{
    public required string FileName { get; set; }
    public string? Hash { get; set; }
    public DateTime? LastModified { get; set; }
}