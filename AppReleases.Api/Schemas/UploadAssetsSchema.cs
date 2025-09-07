using AppReleases.Models;

namespace AppReleases.Api.Schemas;

public class UploadAssetsSchema
{
    public required IEnumerable<AssetInfo> Assets { get; set; }

    public required IFormFile Zip { get; set; }
}