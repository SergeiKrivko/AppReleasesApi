namespace Installer.Shared.Schemas;

public class AssetSchema
{
    public required string FileName { get; set; }
    public string? FileHash { get; set; }
}