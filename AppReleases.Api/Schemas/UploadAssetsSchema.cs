using AppReleases.Core.Models;

namespace AppReleases.Api.Schemas;

public class UploadAssetsSchema
{
    public required AssetInfo[] Assets { get; set; }
    public required IFormFile Zip { get; set; }
}