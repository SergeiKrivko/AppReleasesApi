using AppReleases.Models;

namespace AppReleases.Core.Abstractions;

public interface IReleaseService
{
    public Task<Release> GetReleaseByIdAsync(Guid releaseId);
    public Task<Release?> GetLatestReleaseAsync(Guid branchId, string platform);
    public Task<IEnumerable<Release>> GetAllReleasesOfApplicationAsync(Guid applicationId);
    public Task<ReleaseDifference> GetReleaseDifferenceAsync(AssetInfo[] assets);
    public Task<Release> CreateReleaseAsync(Guid branchId, string? platform, Version version);
    public Task UploadAssetsAsync(Guid releaseId, AssetInfo[] assets, Stream zip);
    public Task<string> PackAssetsAsync(Guid releaseId);
}