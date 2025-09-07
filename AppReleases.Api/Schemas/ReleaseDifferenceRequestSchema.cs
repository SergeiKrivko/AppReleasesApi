using AppReleases.Core.Models;

namespace AppReleases.Api.Schemas;

public class ReleaseDifferenceRequestSchema
{
    public required string ApplicationKey { get; set; }
    public AssetInfo[] Assets { get; set; } = [];
}