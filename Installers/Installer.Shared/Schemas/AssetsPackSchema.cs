namespace Installer.Shared.Schemas;

public class AssetsPackSchema
{
    public required string Url { get; set; }
    public AssetSchema[] ModifiedAssets { get; set; } = [];
    public AssetSchema[] DeletedAssets { get; set; } = [];
}