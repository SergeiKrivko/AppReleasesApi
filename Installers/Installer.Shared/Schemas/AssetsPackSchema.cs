namespace Installer.Shared.Schemas;

public class AssetsPackSchema
{
    public required string Url { get; set; }
    public string[] ModifiedAssets { get; set; } = [];
    public string[] DeletedAssets { get; set; } = [];
}