using AppReleases.Models;

namespace AppReleases.Core.Abstractions;

public interface IReleaseService
{
    public Task<Release> GetReleaseByIdAsync(Guid releaseId);
    public Task<Release?> GetLatestReleaseAsync(Guid branchId, string platform);
    public Task<IEnumerable<Release>> GetAllReleasesOfBranchAsync(Guid branchId);
    public Task<IEnumerable<Release>> GetAllReleasesOfApplicationAsync(Guid applicationId);
    public Task<ReleaseDifference> GetReleaseDifferenceAsync(AssetInfo[] assets);
    public Task<Release> CreateReleaseAsync(Guid branchId, string? platform, Version version);
    public Task UpdateReleaseAsync(Guid releaseId, string? description);
    public Task DeleteReleaseAsync(Guid release);
    public Task UploadAssetsAsync(Guid releaseId, AssetInfo[] assets, Stream zip);
    public Task<string> PackAssetsAsync(Guid releaseId);
    public Task<AssetsPack> PackAssetsAsync(Guid releaseId, AssetInfo[] existingAssets);
    public Task<IEnumerable<AssetInfo>> ListAssetsAsync(Guid releaseId);
}