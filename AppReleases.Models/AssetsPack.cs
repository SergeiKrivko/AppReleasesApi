namespace AppReleases.Models;

public class AssetsPack
{
    public required string Url { get; set; }
    public string[] ModifiedAssets { get; set; } = [];
    public string[] DeletedAssets { get; set; } = [];
}