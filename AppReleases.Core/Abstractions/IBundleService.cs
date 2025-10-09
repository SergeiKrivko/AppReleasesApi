using AppReleases.Models;

namespace AppReleases.Core.Abstractions;

public interface IBundleService
{
    public Task<Bundle> GetBundleByIdAsync(Guid id);
    public Task<IEnumerable<Bundle>> GetAllBundlesAsync(Guid releaseId);
    public Task<Bundle?> FindBundleAsync(Guid releaseId, string fileName);
    public Task<Bundle> CreateBundleAsync(Guid releaseId, string fileName, Stream stream);
    public Task DeleteBundleAsync(Guid bundleId);
    public Task<string> GetDownloadUrlAsync(Guid bundleId);
}