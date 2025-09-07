using AppReleases.Core.Models;

namespace AppReleases.Core.Abstractions;

public interface IReleaseService
{
    public Task<Release> GetReleaseByIdAsync(Guid releaseId);
    public Task<Release?> GetLatestReleaseAsync(Guid branchId, string platform);
    public Task<ReleaseDifference> GetReleaseDifferenceAsync(AssetInfo[] assets);
}