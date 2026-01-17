namespace Installer.Shared.Schemas;

public class AssetSchema
{
    public required string FileName { get; set; }
    public string? FileHash { get; set; }
}

public class InstalledAssetSchema : AssetSchema
{
    public required string InstalledFileName { get; set; }
}