using AppReleases.Core.Abstractions;
using AppReleases.Core.Models;

namespace AppReleases.Application.Services;

public class ReleaseService(IReleaseRepository releaseRepository, IAssetRepository assetRepository) : IReleaseService
{
    public Task<Release> GetReleaseByIdAsync(Guid releaseId)
    {
        return releaseRepository.GetReleaseByIdAsync(releaseId);
    }

    public Task<Release?> GetLatestReleaseAsync(Guid branchId, string platform)
    {
        return releaseRepository.GetLatestReleaseAsync(branchId, platform);
    }

    public async Task<ReleaseDifference> GetReleaseDifferenceAsync(Guid releaseId, AssetInfo[] assets)
    {
        var toUpload = new List<string>();
        foreach (var asset in assets)
        {
            var existing = await assetRepository.FindAssetAsync(asset.FileName, asset.FileHash);
            if (existing == null)
                toUpload.Add(asset.FileName);
        }

        return new ReleaseDifference
        {
            FilesToUpload = toUpload.ToArray(),
        };
    }
}