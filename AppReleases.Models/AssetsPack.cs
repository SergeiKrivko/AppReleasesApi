namespace AppReleases.Models;

public class AssetsPack
{
    public required string Url { get; set; }
    public AssetInfo[] ModifiedAssets { get; set; } = [];
    public AssetInfo[] DeletedAssets { get; set; } = [];
}