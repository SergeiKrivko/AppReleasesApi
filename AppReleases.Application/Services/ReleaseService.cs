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

    public async Task<ReleaseDifference> GetReleaseDifferenceAsync(AssetInfo[] assets)
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

    public async Task<Release> CreateReleaseAsync(Guid branchId, string platform, Version version)
    {
        var release = new Release
        {
            Id = Guid.NewGuid(),
            BranchId = branchId,
            Platform = platform,
            Version = version,
            CreatedAt = DateTime.UtcNow
        };
        await releaseRepository.CreateReleaseAsync(release);
        return release;
    }
}